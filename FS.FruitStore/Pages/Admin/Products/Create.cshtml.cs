using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
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
        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile ImgUp { get; set; }

        [BindProperty]
        public IFormFile ImgUp1 { get; set; }

      
        #region Discount
        //لیست رو پر میکنه
        public SelectList Discounts { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedDiscount { get; set; }

        public string CurrentDiscount { get; set; }
        #endregion
        public async Task<IActionResult> OnGetAsync()
        {
            initSelectoptions();
            return Page();

        }
        async void initSelectoptions()
        {

            //Discounts
            var AllDiscounts = await _db.Discounts
                .ToListAsync();
            if (AllDiscounts != null)
                Discounts = new SelectList(AllDiscounts, "Percent", "Percent");


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                initSelectoptions();
                return Page();
            }

            var checkIfRedundunt = await _db
                .Products
                .Where(a => a.Name == Product.Name)
                .FirstOrDefaultAsync();

            if (checkIfRedundunt != null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "محصولی با همین نام موجود است";
                #endregion
                return RedirectToPage("Index");
            }

            string SaveDir = $"wwwroot/ProductImages";

            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);

            if (ImgUp != null)
            {
                // بررسی فایل ورودی
                if (ImageFormats.CheckFormats(Path.GetExtension(ImgUp.FileName)) == null)
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "لطفا عکس وارد کنید";
                    #endregion
                    return Page();
                }

                Product.ProductPic = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, Product.ProductPic);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }
            if (ImgUp1 != null)
            {
                // بررسی فایل ورودی
                if (ImageFormats.CheckFormats(Path.GetExtension(ImgUp1.FileName)) == null)
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "لطفا عکس وارد کنید";
                    #endregion
                    return Page();
                }

                Product.ProductPic2 = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp1.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, Product.ProductPic2);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp1.CopyTo(filestream);
                }
            }

            if (SelectedDiscount != null && SelectedDiscount != "بدون تخفیف")
            {
                Product.Discount = Convert.ToDouble(SelectedDiscount);
            }
            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name);
            Product.UserId = userId.Id;
            _db.Products.Add(Product);
            await _db.SaveChangesAsync();

            var productId = await _db
                .Products
                .OrderByDescending(a => a.CreateDate)
                .FirstOrDefaultAsync();

            
            await _db.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./AddCategoryToProduct", new { Id = Product.ProductId });
        }

    }
}
