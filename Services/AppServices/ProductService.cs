using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.AppServices
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Unit>> GetUnitsByProductAsync(int productId)
        {
            return await _db.UnitToProducts
                .Where(a => a.ProductId == productId)
                .Select(a => a.Unit)
                .ToListAsync();
        }
    }
}