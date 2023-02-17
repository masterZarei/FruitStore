using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.AboutUs_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class AboutUs_mgmtModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public AboutUs_mgmtModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public AboutUs AboutUs { get; set; }

        [BindProperty]
        public IFormFile ImgUp { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {


            AboutUs = await _db.AboutUs.FirstOrDefaultAsync();

            if (AboutUs == null)
            {
                _db.Add(new AboutUs()
                        {
                            Img = "",
                            Text = ""
                        });
                await _db.SaveChangesAsync();
                AboutUs = await _db.AboutUs.FirstOrDefaultAsync();
            }


            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return Page();
            }


            if (ImgUp != null)
            {
                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences");
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
                if (!string.IsNullOrEmpty(AboutUs.Img))
                {
                    string deletePath = Path.Combine(DirectoryPath, AboutUs.Img);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }


                AboutUs.Img = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(DirectoryPath, AboutUs.Img);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }



            _db.Update(AboutUs);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "با موفقیت انجام شد";
            #endregion
            return RedirectToPage("Index");
        }

    }
}
