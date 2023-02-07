using FS.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Roles;

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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return BadRequest();
            var userId = claim.Value;


            var crntFactor = _db.Factors
                .Where(a => a.FactorId == Id)
                .Include(a => a.FactorDetails)
                .ThenInclude(a => a.Product)
                .FirstOrDefault();

            if (crntFactor != null)
            {
                crntFactor.WillDeliver_Date = System.DateTime.Now.AddDays(SD.CountOfDaysPackageWillDeliver);
                crntFactor.Send_Date = System.DateTime.Now.AddDays(2);
                crntFactor.Post_Type = "سفارشی";
                
                _db.Update(crntFactor);
                await _db.SaveChangesAsync();

                return Page();

            }

            return Page(); // And send Errors

        }
    }
}
