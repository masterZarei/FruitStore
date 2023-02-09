using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    [Authorize(Roles =SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public DeleteModel(FS.DataAccess.ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "شناسه نا معتبر است";
                #endregion
            }

            BenefitBar = await _context.BenefitBars.FindAsync(id);

            if (BenefitBar != null)
            {
                _context.BenefitBars.Remove(BenefitBar);
                await _context.SaveChangesAsync();
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./Index");
        }
    }
}
