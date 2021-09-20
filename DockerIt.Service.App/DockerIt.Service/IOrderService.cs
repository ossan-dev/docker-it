using DockerIt.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DockerIt.Service
{
    public interface IOrderService
    {
        Task<ICollection<Order>> GetAll(int page = 0, int take = 10);
        Task<Order> GetbyId(int id);
    }
}