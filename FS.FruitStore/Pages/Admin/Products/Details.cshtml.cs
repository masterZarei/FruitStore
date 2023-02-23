using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Products
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;

        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<Category> ProdCats { get; set; }

        [BindProperty]
        public List<Unit> ProdUnits { get; set; }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {

            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            Product = await _context
                .Products
                .FirstOrDefaultAsync(u => u.ProductId == Id);


            if (Product == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            ProdCats = await (from a in _context.Categories
                              join b in _context.CategoryToProducts on a.Id equals b.CategoryId
                              where b.ProductId == Product.ProductId
                              select a).ToListAsync();

            ProdUnits = await (from a in _context.Units
                               join b in _context.UnitToProducts on a.Id equals b.UnitId
                               where b.ProductId == Id
                               select a).ToListAsync();

            return Page();
        }
    }
}
