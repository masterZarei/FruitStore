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

            Slider = await _context
                .Sliders
                .ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostRemove(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }

            var thisSlider = await _context
                .Sliders
                .Where(a => a.Id == Id)
                .FirstOrDefaultAsync();

            if (thisSlider == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return Page();
            }

            string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", (string.IsNullOrEmpty(thisSlider.Img) ? "" : thisSlider.Img));
            if (System.IO.File.Exists(deletePath))
                System.IO.File.Delete(deletePath);

            _context.Remove(thisSlider);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
        
    }
}
