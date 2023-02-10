using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages
{
    public class AllProductsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllProductsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Cat
        [BindProperty]
        public List<Category> Category { get; set; }
        //لیست رو پر میکنه
        public SelectList Cats { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedCat { get; set; }
        #endregion

        public IList<Product> Product { get; set; }

        public async Task<IActionResult> OnGetAsync(string SC)
        {
            Category = await (from a in _context.Categories
                        select a).ToListAsync();

            if (Category != null)
            {
                Cats = new SelectList(Category, "Name", "Name");
            }
            if (SC != null)
            {
                Product = await (from p in _context.Products
                           join ctp in _context.CategoryToProducts on p.ProductId equals ctp.ProductId
                           where ctp.Category.Name == SC && ctp.Product.ProductId == p.ProductId && p.isVerified == true
                           select p).ToListAsync();
                return Page();
            }

            Product = await _context.Products
                .Where(a => a.isVerified).ToListAsync();

            SelectedCat = SC;
            return Page();

        }
        public IActionResult OnPostFilCat()
        {
            if (SelectedCat != null)
            {
                return RedirectToPage("Product", new { SC = SelectedCat });
            }
            return Page();
        }


    }
}
