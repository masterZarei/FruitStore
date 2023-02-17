using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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


        //Made THis method Async
        public async Task<IActionResult> OnGetAsync(int Id)
        {
            ProdCats = await (from a in _db.Categories
                        join b in _db.CategoryToProducts on a.Id equals b.CategoryId
                        where b.ProductId == Id
                        select a).ToListAsync();

            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
            return RedirectToPage("Index");
            }

            Category = (from a in _db.Categories
                        where !ProdCats.Contains(a)
                        select a).ToList();

            if (Category.Count == 0) return RedirectToPage("Index");


            if (Category != null)
                Cats = new SelectList(Category, "Name", "Name");



            Product = _db
                .Products
                .Where(a => a.ProductId == Id)
                .FirstOrDefault();

            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }
            return RedirectToPage("./Index");

        }
        public async Task<IActionResult> OnPostRemoveCat(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return Page();
            }

            var findCat = await _db.CategoryToProducts
                .FirstOrDefaultAsync(a => a.CategoryId == Id && a.ProductId == Product.ProductId);

            if (findCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            _db.Remove(findCat);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });

        }
        public async Task<IActionResult> OnPostAddCat()
        {
            if (SelectedCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var findCat = await _db
                .Categories
                .Where(a => a.Name == SelectedCat)
                .FirstOrDefaultAsync();

            if (findCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            CategoryToProduct ctp = new CategoryToProduct()
            {
                CategoryId = findCat.Id,
                ProductId = Product.ProductId
            };


            _db.Add(ctp);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });

        }
    }
}
