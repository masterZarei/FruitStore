using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Payments
{
    [Authorize]
    public class ConfirmInformationModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ConfirmInformationModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public User apUser { get; set; }

        [BindProperty]
        public string postType { get; set; }
        [BindProperty]
        public string Description { get; set; }
        public async Task<IActionResult> OnGet(int Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            if (Id == 0)
                return NotFound();
            var factor = _db.Factors
                .Where(a => a.FactorId == Id &&
                a.UserId == userId && !a.IsFinally &&
                a.FactorDetails.Count>1)
                .FirstOrDefault();

             apUser = _db.Users.Where(a => a.Id == userId).FirstOrDefault();

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            var currentUser = _db.Users.FirstOrDefault(a=>a.Id == userId);

            var factor = _db.Factors
                .Where(a => a.UserId == userId && !a.IsFinally)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(apUser.PostalCode)
                || (string.IsNullOrEmpty(apUser.Address)))
            {
                return RedirectToPage("ConfirmInformation", new { Id = factor.FactorId });
            }
            else
            {
                currentUser.PostalCode = apUser.PostalCode;
                currentUser.Address = apUser.PostalCode;
                currentUser.PostalCode = apUser.PostalCode;

                factor.Post_Type = postType;
                factor.Description = Description;

                _db.Update(currentUser);
                _db.Update(factor);
                await _db.SaveChangesAsync();
                return RedirectToPage("PaymentInfo", new { Id = factor.FactorId });

            }

        }
        //public async Task<IActionResult> OnPostAsync(int Id)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (claim == null)
        //        return Redirect("/Identity/Account/Login");
        //    var userId = claim.Value;

        //    var currentOrder = await _db.Factors.Where(o => o.UserId == userId && !o.IsFinally && o.FactorId == Id)
        //        .Include(o => o.FactorDetails)
        //        .ThenInclude(c => c.Product).FirstOrDefaultAsync();

        //    int FullPrice = Convert.ToInt32(currentOrder.FactorDetails.Sum((s => s.Count * s.Price)));

        //    var payment = await new ZarinpalSandbox.Payment(FullPrice).PaymentRequest("عنوان", $"https://localhost:44371/Payment/OnlinePayment?Id={currentOrder.FactorId}&");


        //    if (payment.Status == 100)
        //    {

        //        return Redirect(payment.Link);
        //    }
        //    else
        //    {
        //        //return errorPage;
        //        return RedirectToAction("ErrorPage", "Home");
        //    }

        //}
    }
}
