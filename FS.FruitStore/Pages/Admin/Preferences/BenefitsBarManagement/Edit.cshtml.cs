using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    [Authorize(Roles =SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public BenefitBar BenefitBar { get; set; }
        [BindProperty]
        public IFormFile ImgUp { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            BenefitBar = await _context
                .BenefitBars
                .FirstOrDefaultAsync(m => m.Id == id);

            if (BenefitBar == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

                if (!string.IsNullOrEmpty(BenefitBar.Img))
                {
                    string deletePath = Path.Combine(DirectoryPath, BenefitBar.Img);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }


                BenefitBar.Img = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(DirectoryPath, BenefitBar.Img);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }

            _context.Attach(BenefitBar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenefitBarExists(BenefitBar.Id))
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.IDINVALID;
                    #endregion
                    return RedirectToPage("/NotFound");
                }
                else
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.IDINVALID;
                    #endregion
                    return RedirectToPage("/NotFound");
                }
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./Index");
        }

        private bool BenefitBarExists(int id)
        {
            return _context.BenefitBars.Any(e => e.Id == id);
        }
    }
}
