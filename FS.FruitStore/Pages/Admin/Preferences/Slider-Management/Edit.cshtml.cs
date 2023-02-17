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

namespace FS.FruitStore.Pages.Admin.Preferences.Slider_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Slider Slider { get; set; }

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

            Slider = await _context
                .Sliders
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Slider == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }
            try
            {
                var currentSlider = _context.Sliders.FindAsync(id).Result;

                string SaveDir = $"wwwroot/img";

                if (!Directory.Exists(SaveDir))
                    Directory.CreateDirectory(SaveDir);

                currentSlider.Caption = Slider.Caption;
                currentSlider.Link = Slider.Link;

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

                    currentSlider.Img = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                    string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, currentSlider.Img);
                    using (var filestream = new FileStream(savepath, FileMode.Create))
                    {
                        ImgUp.CopyTo(filestream);
                    }
                }


                _context.Sliders.Update(currentSlider);
                await _context.SaveChangesAsync();

                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
                return RedirectToPage("Index");

            }
            catch (Exception)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return RedirectToPage("Index");

            }

        }

    }
}
