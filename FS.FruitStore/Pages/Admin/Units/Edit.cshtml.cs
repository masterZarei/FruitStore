using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Units
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public EditModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Unit Unit { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            Unit = await _context
                .Units
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Unit == null)
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


            _context.Attach(Unit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitExists(Unit.Id))
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.NOTFOUND;
                    #endregion
                    return RedirectToPage("/NotFound");
                }
                else
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                    #endregion
                }
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./Index");
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.Id == id);
        }
    }
}
