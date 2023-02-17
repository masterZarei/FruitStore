using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

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

            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;


            Factor = await _db.Factors.Where(o => o.UserId == userId && o.IsFinally == false)
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
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var factorDetail = _db.FactorDetails.Find(DetailId);
            if (factorDetail.Count > 1)
            {
                factorDetail.Count -= 1;
                _db.Update(factorDetail);
                await _db.SaveChangesAsync();
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
                return RedirectToPage("ShoppingCart");
            }
            _db.Remove(factorDetail);
            var checkFactor = _db.Factors
                .Include(a => a.FactorDetails)
                .ThenInclude(a => a.Product)
                .Where(a => a.FactorId == factorDetail.FactorId)
                .FirstOrDefault();
            int check = checkFactor
                .FactorDetails
                .Where(a => a.Product.Count > 0)
                .ToList()
                .Count;

            if (check < 1)
            {
                _db.Remove(checkFactor);
            }
            await _db.SaveChangesAsync();


            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostAddToCart(int DetailId)
        {
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var factorDetail = await _db.FactorDetails
                .Include(a => a.Product)
                .Where(a => a.DetailId == DetailId)
                .FirstOrDefaultAsync();

            if (factorDetail.Count <= factorDetail.Product.Count - 1)
            {
                factorDetail.Count += 1;

                _db.Update(factorDetail);
                await _db.SaveChangesAsync();
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
                return RedirectToPage("ShoppingCart");
            }
            else
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "موجودیت محصول کمتر از مقدار خواسته شده می باشد.";
                #endregion
                return RedirectToPage("ShoppingCart");
            }

        }
        public async Task<IActionResult> OnPostRemoveAllCart(int OrderId)
        {
            if (OrderId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var factor = _db.Factors.Find(OrderId);
            if (factor == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion               
                return RedirectToPage("/NotFound");

            }
            _db.Factors.Remove(factor);

            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostRemoveThisCart(int DetailId)
        {
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var orderDetail = _db.FactorDetails.Find(DetailId);
            if (orderDetail == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var factor = await _db.Factors
                .Include(a => a.FactorDetails)
                .Where(a => a.FactorId == orderDetail.FactorId).FirstOrDefaultAsync();

            if (factor == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            if (factor.FactorDetails.Count <= 1)
            {
                _db.Remove(factor);
                await _db.SaveChangesAsync();
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
                return RedirectToPage("ShoppingCart");
            }

            _db.Remove(orderDetail);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
    }
}
