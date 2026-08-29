using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities
{

    public class GetProductInfo
    {
        private readonly ApplicationDbContext _db;

        public GetProductInfo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Unit>> GetUnitAsync(int productId)
        {
            var data = await _db.UnitToProducts
                 .Where(a => a.ProductId == productId)
                 .Include(u => u.Unit)
                 .ToListAsync();

            return data.Select(a => a.Unit).ToList();
        }


    }
}
