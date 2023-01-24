using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Products
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            // Units
            var AllUnits = _db.Units.ToList();
            if (AllUnits != null)
            {
                Units = new SelectList(AllUnits, "Name", "Name");
            }

            return Page();

        }
        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile ImgUp { get; set; }

        [BindProperty]
        public IFormFile ImgUp1 { get; set; }

        #region Unit
        //لیست رو پر میکنه
        public SelectList Units { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedUnit { get; set; }

        public string CurrentUnit { get; set; }
        #endregion

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var checkIfRedundunt = _db.Products.Where(a => a.Name == Product.Name).FirstOrDefault();
            if(checkIfRedundunt!=null)
            {
                TempData["Error"] = " محصولی با همین نام موجود است.";
                return RedirectToPage("Index");
            }

            string SaveDir = $"wwwroot/ProductImages";

            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);

            if (ImgUp != null)
            {
                Product.ProductPic = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, Product.ProductPic);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }
            if (ImgUp1 != null)
            {
                Product.ProductPic2 = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp1.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, Product.ProductPic2);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp1.CopyTo(filestream);
                }
            }

           
           
            _db.Products.Add(Product);
            await _db.SaveChangesAsync();

            var productId = _db.Products.OrderByDescending(a => a.CreateDate).FirstOrDefault();
            var UnitToAssign = _db.Units.FirstOrDefault(a => a.Name == SelectedUnit);
            var newUnit = new UnitToProduct()
            {
                ProductId = Product.ProductId,
                UnitId = UnitToAssign.Id,
                IsDeleted = false

            };
            _db.Add(newUnit);
            await _db.SaveChangesAsync();

            return RedirectToPage("./AddCategoryToProduct", new { Id = Product.ProductId });
            //return RedirectToPage("./Index");
        }

    }
}
