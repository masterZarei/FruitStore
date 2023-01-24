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
using Utilities.Convertors;
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            if (id == null)
            {
                return NotFound();
            }

            Slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (Slider == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
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
                    currentSlider.Img = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                    string savepath = Path.Combine(Directory.GetCurrentDirectory(), SaveDir, currentSlider.Img);
                    using (var filestream = new FileStream(savepath, FileMode.Create))
                    {
                        ImgUp.CopyTo(filestream);
                    }
                }


                _context.Sliders.Update(currentSlider);
                await _context.SaveChangesAsync();

                return RedirectToPage("Index");

            }
            catch (Exception)
            {

                return RedirectToPage("Index");

            }






        }

    }
}
