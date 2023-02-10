using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public BenefitBar BenefitBar { get; set; }


        [BindProperty]
        public IFormFile ImgUp { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return Page();
            }

            if (ImgUp != null)
            {
                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences");
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);


                BenefitBar.Img = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(DirectoryPath, BenefitBar.Img);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }

            _context.BenefitBars.Add(BenefitBar);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "با موفقیت انجام شد";
            #endregion
            return RedirectToPage("./Index");
        }
    }
}
