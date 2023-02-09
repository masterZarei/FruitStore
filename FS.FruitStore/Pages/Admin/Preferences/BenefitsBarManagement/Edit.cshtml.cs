using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    [Authorize(Roles =SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public EditModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BenefitBar BenefitBar { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
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
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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
                    return NotFound();
                }
                else
                {
                    throw;
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
