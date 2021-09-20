using DockerIt.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DockerIt.Service
{
    public interface IOrderService
    {
        Task<ICollection<Order>> GetOrders();
    }
}