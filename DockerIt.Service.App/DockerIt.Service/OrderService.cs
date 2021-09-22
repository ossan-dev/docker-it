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
            var orders = await _dbConnection.QueryAsync<Order>(sql, param: parameters, commandTimeout: 900).ConfigureAwait(false);
            return orders.ToList();
        }

        public async Task<Order> GetbyId(int id)
        {
            var parameters = new { OrderId = id };
            string sql = "SELECT * FROM [Sales].[Orders] WHERE OrderId = @OrderId";
            var order = await _dbConnection.QuerySingleOrDefaultAsync<Order>(sql: sql, param: parameters, commandTimeout: 900).ConfigureAwait(false);
            return order;
        }

        public async Task<int> Create(Order order)
        {
            string sql = "INSERT INTO [Sales].[Orders] ([CustomerID], [SalespersonPersonID], [ContactPersonID], [OrderDate], [ExpectedDeliveryDate], [IsUndersupplyBackordered], [LastEditedBy]) VALUES (@CustomerID, @SalespersonPersonID, @ContactPersonID, @OrderDate, @ExpectedDeliveryDate, @IsUndersupplyBackordered, @LastEditedBy)";
            var affRows = await _dbConnection.ExecuteAsync(sql: sql, new { CustomerId = order.CustomerID, SalespersonPersonID = order.SalespersonPersonID, ContactPersonID = order.ContactPersonID, OrderDate = order.OrderDate, ExpectedDeliveryDate = order.ExpectedDeliveryDate, IsUndersupplyBackordered = order.IsUndersupplyBackordered, LastEditedBy = order.LastEditedBy }).ConfigureAwait(false);
            return affRows;
        }
        
        public async Task<int> Update(Order order)
        {
            string sql = @"
                        UPDATE [Sales].[Orders] 
                            SET [CustomerID] = @CustomerId,
                                [SalespersonPersonID] = @SalespersonPersonID,                         [ContactPersonID] = @ContactPersonID,
                                [OrderDate] = @OrderDate,
                                [ExpectedDeliveryDate] = @ExpectedDeliveryDate,                       [IsUndersupplyBackordered] = @IsUndersupplyBackordered,               [LastEditedBy] = @LastEditedBy
                                WHERE OrderID = @OrderID";
            var affRows = await _dbConnection.ExecuteAsync(sql: sql, order).ConfigureAwait(false);
            return affRows;
        }

        public async Task<int> Delete(int id)
        {
            // TODO: apply the on delete cascade on FK constraint of Orders table
            string sql = "DELETE FROM [Sales].[Orders] WHERE OrderID = @OrderID";
            var affRows = await _dbConnection.ExecuteAsync(sql, new { OrderID = id }).ConfigureAwait(false);
            return affRows;
        }
    }
}
