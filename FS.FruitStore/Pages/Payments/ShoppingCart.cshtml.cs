using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Payments
{
    [Authorize]
    public class ShoppingCartModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Factor Factor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;


            Factor = await _db.Factors.Where(o => o.UserId == userId && o.IsFinally==false)
                .Include(o => o.FactorDetails)
                .ThenInclude(c => c.Product).FirstOrDefaultAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int Id)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //if (claim == null)
            //    return Redirect("/Identity/Account/Login");
            //var userId = claim.Value;

            //var currentOrder = await _db.Factors.Where(o => o.UserId == userId && !o.IsFinally && o.FactorId == Id)
            //    .Include(o => o.FactorDetails)
            //    .ThenInclude(c => c.Product).FirstOrDefaultAsync();

            //int FullPrice = Convert.ToInt32(currentOrder.FactorDetails.Sum((s => s.Count * s.Price)));

            //var payment = await new ZarinpalSandbox.Payment(FullPrice).PaymentRequest("عنوان", $"https://localhost:44371/Payment/OnlinePayment?Id={currentOrder.FactorId}&");


            //if (payment.Status == 100)
            //{

            //    return Redirect(payment.Link);
            //}
            //else
            //{
            //    //return errorPage;
            //    return RedirectToAction("ErrorPage", "Home");
            //}
            return Redirect("ConfirmInformation");

        }
        public async Task<IActionResult> OnPostRemoveCart(int DetailId)
        {
            var factorDetail = _db.FactorDetails.Find(DetailId);
            if (factorDetail.Count > 1)
            {
                factorDetail.Count -= 1;
                _db.Update(factorDetail);
                _db.SaveChanges();

                return RedirectToPage("ShoppingCart");
            }
            _db.Remove(factorDetail);
            _db.SaveChanges();
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostAddToCart(int DetailId)
        {
            var factorDetail = _db.FactorDetails
                .Include(a=>a.Product)
                .Where(a=>a.DetailId == DetailId)
                .FirstOrDefault();

            if (factorDetail.Count <= factorDetail.Product.Count-1)
            {
                factorDetail.Count += 1;

                _db.Update(factorDetail);
                _db.SaveChanges();
                return RedirectToPage("ShoppingCart");

            }
            else
            return RedirectToPage("ShoppingCart");

               // اینجا به کاربر پیغام بفرست


        }
        public async Task<IActionResult> OnPostRemoveAllCart(int OrderId)
        {
            var factor = _db.Factors.Find(OrderId);
            _db.Factors.Remove(factor);

            _db.SaveChanges();
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostRemoveThisCart(int DetailId)
        {
            var orderDetail = _db.FactorDetails.Find(DetailId);
            var factor = _db.Factors
                .Include(a=>a.FactorDetails)
                .Where(a => a.FactorId == orderDetail.FactorId).FirstOrDefault();

            if (factor.FactorDetails.Count<=1)
            {
                _db.Remove(factor);
                _db.SaveChanges();
                return RedirectToPage("ShoppingCart");
            }

            _db.Remove(orderDetail);
            _db.SaveChanges();
            return RedirectToPage("ShoppingCart");
        }
    }
}
