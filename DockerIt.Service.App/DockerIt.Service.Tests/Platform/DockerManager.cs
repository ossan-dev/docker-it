using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DockerIt.Service.Tests.Platform
{
    public class DockerManager : IDisposable
    {
        private readonly DockerClient _dockerClient;

        public DockerManager()
        {
            var dockerUri = (Environment.OSVersion.Platform == PlatformID.Win32NT)
               ? "npipe://./pipe/docker_engine"
               : "unix:///var/run/docker.sock";
            _dockerClient = new DockerClientConfiguration(new Uri(dockerUri))
                .CreateClient();
        }

        public async Task<string> StartContainerAsync(string imgName)
        {
            // create the container
            var freePort = GetFreePort();

            var sqlContainer = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Name = "wwi-db",
                Image = "ivanpesentidev/wwi-db:1.0.0",
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "1433/tcp",
                            new PortBinding[]
                            {
                                new PortBinding
                                {
                                    HostPort = freePort
                                }
                            }
                        }
                    }
                }
            }, cancellationToken: CancellationToken.None);

            await _dockerClient.Containers.StartContainerAsync(sqlContainer.ID, new ContainerStartParameters());
            await WaitUntilDatabaseAvailableAsync(freePort);
            return freePort;
        }

        public async Task CleanupRunningContainers(string sqlContainerPrefix)
        {
            var runningContainers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());

            foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(sqlContainerPrefix))))
            {
                // stop all containers that are running more than an hour as they probably failed to be removed
                if (runningContainer.Created < DateTime.Now.AddHours(-1))
                {
                    try
                    {
                        await EnsureDockerStoppedAndRemovedAsync(runningContainer.ID);
                    }
                    catch
                    {
                        // ignore failure on try to stop containers
                    }
                }
            }
        }

        public async Task<int> GetNumOfContainerByName(string containerPrefix)
        {
            int numContainers = 0;
            var runningContainers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());
            foreach (var container in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(containerPrefix))))
            {
                numContainers++;
            }

            return numContainers;
        }

        public void Dispose()
        {
            if (_dockerClient != null)
            {
                CleanupRunningContainers("wwi-db").GetAwaiter().GetResult();
                _dockerClient.Dispose();            
            }
        }

        public async Task EnsureDockerStoppedAndRemovedAsync(string dockerContainerId)
        {
            await _dockerClient.Containers.StopContainerAsync(dockerContainerId, new ContainerStopParameters());
            await _dockerClient.Containers.RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
        }

        #region private methods

        private async Task WaitUntilDatabaseAvailableAsync(string databasePort)
        {
            var start = DateTime.UtcNow;
            const int maxWaitTimeSeconds = 90;
            var connectionEstablished = false;
            while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
            {
                try
                {
                    var sqlConnectionString = $"Data Source=localhost,{databasePort};Integrated Security=False;User ID=sa;Password=abcd1234ABCD!";
                    using var sqlConnection = new SqlConnection(sqlConnectionString);
                    await sqlConnection.OpenAsync();
                    connectionEstablished = true;
                }
                catch
                {
                    // if opening the SQL connection fails, SQL Server is not ready yet
                    await Task.Delay(500);
                }
            }

            if (!connectionEstablished)
            {
                throw new Exception("Connection to the SQL docker host could not be established within 90 seconds");
            }

            return;
        }

        private string GetFreePort()
        {
            // see this url for refs https://stackoverflow.com/a/150974/4190785
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port.ToString();
        }

        #endregion

    }
}
