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

        public OrderService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ICollection<Order>> GetAll(int page = 1, int take = 10)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"@RowsToSkip",  (page-1) * take},
                {"@RowsToTake",  take}
            };
            
            string sql = "SELECT * FROM [Sales].[Orders] ORDER BY [OrderDate] OFFSET @RowsToSkip ROWS FETCH NEXT @rowsToTake ROWS ONLY;";
            var orders = await _dbConnection.QueryAsync<Order>(sql, param: parameters, commandTimeout: 600).ConfigureAwait(false);
            return orders.ToList();
        }

        public async Task<Order> GetbyId(int id)
        {
            var parameters = new { OrderId = id };
            string sql = "SELECT * FROM [Sales].[Orders] WHERE OrderId = @OrderId";
            var order = await _dbConnection.QuerySingleOrDefaultAsync<Order>(sql: sql, param: parameters, commandTimeout: 600).ConfigureAwait(false);
            return order;
        }


    }
}
