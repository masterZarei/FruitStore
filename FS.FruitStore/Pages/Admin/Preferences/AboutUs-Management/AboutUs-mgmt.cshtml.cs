using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;
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



        public async Task<IActionResult> OnGet()
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion


            AboutUs = _db.AboutUs.FirstOrDefault();

            if (AboutUs == null)
            {
                _db.Add(
                        new AboutUs()
                        {
                            Img = "",
                            Text = ""
                        }
                    );
                _db.SaveChanges();
                AboutUs = _db.AboutUs.FirstOrDefault();
            }


            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();


            if (ImgUp != null)
            {
                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences");
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);

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
            return RedirectToPage("Index");
        }

    }
}
