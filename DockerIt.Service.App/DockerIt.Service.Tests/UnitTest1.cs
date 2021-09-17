using DockerIt.Service.Tests.Platform;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DockerIt.Service.Tests
{
    public class UnitTest1
    {
        private readonly DockerManager _dockerManager;
        private string _hostPort;

        public UnitTest1()
        {
            _dockerManager = new DockerManager();
            _hostPort = _dockerManager.StartContainerAsync("ivanpesentidev/wwi-db:1.0.0").GetAwaiter().GetResult();
        }

        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }
    }
}
