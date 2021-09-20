using Dapper;
using DockerIt.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DockerIt.Service
{
    public class OrderService : IOrderService
    {
        private readonly IDbConnection _dbConnection;

        // TODO: CRUD
        public OrderService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ICollection<Order>> GetOrders()
        {
            string sql = "SELECT TOP 10 * FROM [Sales].[Orders]";
            var orders = await _dbConnection.QueryAsync<Order>(sql, commandTimeout: 600).ConfigureAwait(false);
            return orders.ToList();
        }
    }
}
