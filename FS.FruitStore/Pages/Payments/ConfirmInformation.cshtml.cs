using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Convertors;

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
        public ConfirmInformationVM CIModel { get; set; }



        public async Task<IActionResult> OnGet(int Id)
        {
            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;

            if (Id == 0)
                return NotFound();
            var factor = _db.Factors
                .Where(a => a.FactorId == Id &&
                a.UserId == userId && !a.IsFinally &&
                a.FactorDetails.Count > 1)
                .FirstOrDefault();

            
            CIModel = new ConfirmInformationVM()
            {
                ApplicationUser = _db.Users
                .Where(a => a.Id == userId)
                .FirstOrDefault()
            };
            var AllPostTypes = PostTypes.GetTypes;
            var AllPaymentTypes = PayWays.GetWays;
            CIModel.PostTypes = new SelectList(AllPostTypes, "Value", "Name");
            CIModel.PaymentTypes = new SelectList(AllPaymentTypes, "Value", "Name");

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;

            var currentUser = _db.Users.FirstOrDefault(a => a.Id == userId);



            var factor = _db.Factors
                .Where(a => a.UserId == userId && !a.IsFinally)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(CIModel.ApplicationUser.PostalCode)
                || (string.IsNullOrEmpty(CIModel.ApplicationUser.Address)))
            {
                ModelState.AddModelError(string.Empty, "لطفا کد پستی را وارد کنید!");
                ModelState.AddModelError(string.Empty, "لطفا آدرس را وارد کنید!");
                return RedirectToPage("ConfirmInformation", new { Id = factor.FactorId });

            }
            else
            {
                currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;
                currentUser.Address = CIModel.ApplicationUser.PostalCode;
                currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;

                factor.Post_Type = CIModel.SelectedPostType;
                factor.Payment_Type = CIModel.SelectedPaymentType;
                factor.Description = CIModel.Description;
                factor.WillDeliver_Date = DateTime.Now.AddDays(10);
                factor.Send_Date = DateTime.Now.AddDays(2);


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
