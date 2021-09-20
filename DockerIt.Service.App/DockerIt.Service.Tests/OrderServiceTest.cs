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
        private readonly IOrderService _orderService;
        public OrderServiceTest(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _orderService = new OrderService(new SqlConnection($"Data Source=localhost,{_databaseFixture.SqlPort};Initial Catalog=WideWorldImporters;Integrated Security=False;User ID=sa;Password=abcd1234ABCD!"));
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 15)]
        [InlineData(500, 15)]
        public async Task GetAll_Returns_Exact_Rows_Num(int page, int take)
        {
            // arrange            

            // act
            var orders = await _orderService.GetAll(page: 1, take: take);

            // assert            
            Assert.True(orders.Count <= take);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2000)]
        public async Task GetById_Returns_Exact_Row(int orderId)
        {
            // arrange

            // act
            var order = await _orderService.GetbyId(orderId);

            // assert            
            Assert.Equal(orderId, order.OrderID);
        }

        [Theory]
        [InlineData(100000)]
        public async Task GetById_Returns_Null_When_Not_Found(int orderId)
        {
            // arrange

            // act
            var order = await _orderService.GetbyId(orderId);

            // assert            
            Assert.Null(order);
        }
    }
}
