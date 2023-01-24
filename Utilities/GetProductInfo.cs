using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Unit GetUnit(int productId)
        {
            try
            {

                var data = _db.UnitToProducts
                     .Where(a => a.ProductId == productId)
                     .Include(u => u.Unit)
                     .FirstOrDefault()
                     .Unit;
                if (data == null)
                    return null;
                return data;
            }
            catch
            {
                return null;

            }

        }


    }
}
