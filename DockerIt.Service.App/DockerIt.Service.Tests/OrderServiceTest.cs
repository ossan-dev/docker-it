using DockerIt.Entities;
using DockerIt.Service.Tests.Platform;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace DockerIt.Service.Tests
{
    public class OrderServiceTest : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _connection;
        public OrderServiceTest(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _connection = new SqlConnection($"Data Source=localhost,{_databaseFixture.SqlPort};Initial Catalog=WideWorldImporters;Integrated Security=False;User ID=sa;Password=abcd1234ABCD!");
        }

        [Fact]
        public async Task Test1()
        {
            // arrange
            var orderService = new OrderService(_connection);

            // act
            var orders = await orderService.GetOrders();

            // assert
            Assert.Equal(10, orders.Count);
        }
    }
}
