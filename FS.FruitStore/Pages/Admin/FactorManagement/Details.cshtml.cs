using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Mahshop.Pages.FactorManagement
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Factor Factor { get; set; }

        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            Factor = await _db.Factors
                .Include(a=>a.User)
                .Include(a=>a.FactorDetails)
                .ThenInclude(a=>a.Product)
                .ThenInclude(a=>a.User)
                .Where(a => a.FactorId == Id)
                .OrderByDescending(a=>a.CreateDate)
                .FirstOrDefaultAsync();

            if (Factor == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            return Page();
        }
    }
}
