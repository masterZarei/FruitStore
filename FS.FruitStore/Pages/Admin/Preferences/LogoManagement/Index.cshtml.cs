using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Utilities;

namespace FS.FruitStore.Pages.Admin.Preferences.LogoManagement
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Logo Logo { get; set; }

        [BindProperty]
        public IFormFile ImgUp { get; set; }
        public async Task<IActionResult> OnGet()
        {
            Logo = await _db.Logos
                .FirstOrDefaultAsync();


            if (Logo == null)
            {
                _db.Add(new Logo
                {
                     Image = "",

                });
                _db.SaveChanges();

                Logo = await _db.Logos
                .FirstOrDefaultAsync();
            }

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }

            if (ImgUp != null)
            {
                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image");
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                // بررسی فایل ورودی
                if (ImageFormats.CheckFormats(Path.GetExtension(ImgUp.FileName)) == null)
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "لطفا عکس وارد کنید";
                    #endregion
                    return Page();
                }

                if (!string.IsNullOrEmpty(Logo.Image))
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image", Logo.Image);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }

                

                Logo.Image = "logo" + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image", Logo.Image);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }
            _db.Update(Logo);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return Page();
        }
    }
}
