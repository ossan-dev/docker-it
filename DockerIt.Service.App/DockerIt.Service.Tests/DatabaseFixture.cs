using DockerIt.Service.Tests.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerIt.Service.Tests
{
    public class DatabaseFixture
    {
        private readonly DockerManager _dockerManager;

        public string SqlPort { get; private set; }
        public DatabaseFixture()
        {
            _dockerManager = new DockerManager();
            if (IsNewContainerNeeded().GetAwaiter().GetResult())
                InitializeSqlContainer().GetAwaiter().GetResult();
        }

        public async Task InitializeSqlContainer()
        {
            await _dockerManager.CleanupRunningContainers("wwi-db");
            SqlPort = await _dockerManager.StartContainerAsync("ivanpesentidev/wwi-db:1.0.0");
        }

        public async Task<bool> IsNewContainerNeeded()
        {
            if(await _dockerManager.GetNumOfContainerByName("wwi-db") > 0)
            {
                SqlPort = "57265";
                return false;
            }
            return true;
        }
    }
}
