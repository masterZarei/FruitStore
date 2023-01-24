using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Products
{
    public class AddCategoryToProductModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public AddCategoryToProductModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Category> Category { get; set; }
        //لیست رو پر میکنه
        public SelectList Cats { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedCat { get; set; }
        [BindProperty]
        public List<Category> ProdCats { get; set; }

        [BindProperty]
        public Product Product { get; set; }



        public async Task<IActionResult> OnGetAsync(int Id)
        {
            ProdCats = (from a in _db.Categories
                        join b in _db.CategoryToProducts on a.Id equals b.CategoryId
                        where b.ProductId == Id
                        select a).ToList();

            if (Id == 0)
                return RedirectToPage("Index");

            Category = (from a in _db.Categories
                        where !ProdCats.Contains(a)
                        select a).ToList();

            if (Category.Count == 0) return RedirectToPage("Index");




            if (Category != null)
                Cats = new SelectList(Category, "Name", "Name");



            Product = _db.Products.Where(a => a.ProductId == Id).FirstOrDefault();

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            return RedirectToPage("./Index");

        }
        public async Task<IActionResult> OnPostRemoveCat(int Id)
        {
            if (Id == 0)
            {
                return Page();
            }

            var findCat = _db.CategoryToProducts
                .FirstOrDefault(a => a.CategoryId == Id && a.ProductId == Product.ProductId);

            if (findCat == null)
                return NotFound();

            _db.Remove(findCat);
            await _db.SaveChangesAsync();

            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });

        }
        public async Task<IActionResult> OnPostAddCat()
        {
            if (SelectedCat == null)
            {
                return NotFound();
            }

            var findCat = _db.Categories.Where(a => a.Name == SelectedCat).FirstOrDefault();
            if (findCat == null)
                return NotFound();

            CategoryToProduct ctp = new CategoryToProduct()
            {
                CategoryId = findCat.Id,
                ProductId = Product.ProductId
            };


            _db.Add(ctp);
            await _db.SaveChangesAsync();

            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });

        }
    }
}
