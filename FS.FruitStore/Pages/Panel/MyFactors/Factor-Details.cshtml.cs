using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Panel.MyFactors
{
    [Authorize]
    public class Factor_DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Factor_DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<FactorDetail> FactorDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            FactorDetail = await _db.FactorDetails
                .Include(a=>a.Product)
                .Where(a => a.FactorId == Id)
                .ToListAsync();

            if (FactorDetail == null)
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
