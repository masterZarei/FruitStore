using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Utilities.Roles;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;
using Utilities;

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

        public string ProductUnit;

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            if (Id == 0)
                return NotFound();

            Product = await _context.Products.FirstOrDefaultAsync(u => u.ProductId == Id);
            //Getting unit
            var checkProductUnit = new GetProductInfo(_context).GetUnit(Product.ProductId);

            if(checkProductUnit == null)
                ProductUnit = "0";
            else
                ProductUnit = new GetProductInfo(_context).GetUnit(Product.ProductId).Name;

            if (Product == null)
                return NotFound();

            ProdCats = (from a in _context.Categories
                        join b in _context.CategoryToProducts on a.Id equals b.CategoryId
                        where b.ProductId == Product.ProductId
                        select a).ToList();

            return Page();
        }
    }
}
