using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.Slider_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Slider> Slider { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Slider = await _context.Sliders.ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostRemove(int Id)
        {
            if (Id == 0)
                return NotFound();
            var thisSlider = _context.Sliders.Where(a => a.Id == Id).FirstOrDefault();
            if (thisSlider == null)
                return Page();

            string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", (string.IsNullOrEmpty(thisSlider.Img) ? "" : thisSlider.Img));
            if (System.IO.File.Exists(deletePath))
                System.IO.File.Delete(deletePath);

            _context.Remove(thisSlider);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
        
    }
}
