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
        public IActionResult OnPost(int Id)
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
               await _db.SaveChangesAsync();

                return RedirectToPage("ShoppingCart");
            }
            _db.Remove(factorDetail);
           await _db.SaveChangesAsync();
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostAddToCart(int DetailId)
        {
            var factorDetail = await _db.FactorDetails
                .Include(a=>a.Product)
                .Where(a=>a.DetailId == DetailId)
                .FirstOrDefaultAsync();

            if (factorDetail.Count <= factorDetail.Product.Count-1)
            {
                factorDetail.Count += 1;

                _db.Update(factorDetail);
               await _db.SaveChangesAsync();
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

            await _db.SaveChangesAsync();
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostRemoveThisCart(int DetailId)
        {
            var orderDetail = _db.FactorDetails.Find(DetailId);
            var factor = await _db.Factors
                .Include(a=>a.FactorDetails)
                .Where(a => a.FactorId == orderDetail.FactorId).FirstOrDefaultAsync();

            if (factor.FactorDetails.Count<=1)
            {
                _db.Remove(factor);
               await _db.SaveChangesAsync();
                return RedirectToPage("ShoppingCart");
            }

            _db.Remove(orderDetail);
            await _db.SaveChangesAsync();
            return RedirectToPage("ShoppingCart");
        }
    }
}
