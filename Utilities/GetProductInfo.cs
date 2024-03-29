﻿using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{

    public class GetProductInfo
    {
        private readonly ApplicationDbContext _db;

        public GetProductInfo(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Unit> GetUnit(int productId)
        {
            try
            {

                var data =  _db.UnitToProducts
                     .Where(a => a.ProductId == productId)
                     .Include(u => u.Unit)
                     .ToList();
                if (data == null)
                    return null;

                return (List<Unit>)data.Select(a=>a.Unit);
            }
            catch
            {
                return null;

            }

        }


    }
}
