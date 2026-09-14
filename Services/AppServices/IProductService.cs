using FS.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.AppServices
{
    public interface IProductService
    {
        Task<List<Unit>> GetUnitsByProductAsync(int productId);
    }
}