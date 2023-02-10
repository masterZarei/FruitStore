using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ContactWays> ContactWays { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ContactWays = await _context
                .ContactWays
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

            var item = _context
                .ContactWays
                .Where(a => a.Id == Id)
                .FirstOrDefault();

            if (item == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return Page();
            }


            _context.Remove(item);
            await _context.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}