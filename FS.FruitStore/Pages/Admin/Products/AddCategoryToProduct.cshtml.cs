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
        #region Cat
        [BindProperty]
        public List<Category> Category { get; set; }
        //لیست رو پر میکنه
        public SelectList Cats { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedCat { get; set; }
        [BindProperty]
        public List<Category> ProdCats { get; set; }
        #endregion
        #region Unit
        [BindProperty]
        public List<Unit> Unit { get; set; }
        //لیست رو پر میکنه
        public SelectList Units { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedUnit { get; set; }
        [BindProperty]
        public List<Unit> ProdUnits { get; set; }
        #endregion

        [BindProperty]
        public Product Product { get; set; }


        //Made THis method Async
        public async Task<IActionResult> OnGetAsync(int Id)
        {
            ProdCats = await (from a in _db.Categories
                              join b in _db.CategoryToProducts on a.Id equals b.CategoryId
                              where b.ProductId == Id
                              select a).ToListAsync();

            ProdUnits = await (from a in _db.Units
                               join b in _db.UnitToProducts on a.Id equals b.UnitId
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
            Unit = (from a in _db.Units
                        where !ProdUnits.Contains(a)
                        select a).ToList();

            if (Category.Count == 0 && Unit.Count == 0) return RedirectToPage("Index");


            if (Category != null)
                Cats = new SelectList(Category, "Name", "Name");

            if (Unit != null)
                Units = new SelectList(Unit, "Name", "Name");


            Product = await _db
                .Products
                .Where(a => a.ProductId == Id)
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> OnPostAddUnit()
        {
            if (SelectedUnit == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var findUnit = await _db.Units
                .Where(a => a.Name == SelectedUnit)
                .FirstOrDefaultAsync();
            if (findUnit == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            _db.Add(
                new UnitToProduct()
                {
                    ProductId = Product.ProductId,
                    UnitId = findUnit.Id,
                });
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });
        }
        public async Task<IActionResult> OnPostRemoveUnit(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var findUnit = await _db.UnitToProducts
                .Include(a=>a.Unit)
                .Where(a => a.UnitId == Id && a.ProductId == Product.ProductId)
                .FirstOrDefaultAsync();
            if (findUnit == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            _db.Remove(findUnit);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("AddCategoryToProduct", new { Id = Product.ProductId });
        }
    }
}
