using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace Mahshop.Pages.Payment
{
    [Authorize]
    public class PaymentInfoModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public PaymentInfoModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGet(int Id, string Authority, string Status)
        {

            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;


            var crntFactor = await _db.Factors
                .Where(a => a.FactorId == Id && a.UserId == userId)
                .Include(a => a.FactorDetails)
                .ThenInclude(a => a.Product)
                .FirstOrDefaultAsync();

            if (crntFactor == null)
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
