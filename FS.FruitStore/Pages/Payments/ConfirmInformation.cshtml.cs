using FS.DataAccess;
using FS.Models.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Convertors;
using Utilities.Roles;

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
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }

            var factor = await _db.Factors
                .Where(a => a.FactorId == Id &&
                a.UserId == userId && !a.IsFinally &&
                a.FactorDetails.Count > 1)
                .FirstOrDefaultAsync();

            
            CIModel = new ConfirmInformationVM()
            {
                ApplicationUser = await _db.Users
                .Where(a => a.Id == userId)
                .FirstOrDefaultAsync()
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

            var currentUser = await _db.Users
                .FirstOrDefaultAsync(a => a.Id == userId);



            var factor = await _db.Factors
                .Include(a=> a.FactorDetails)
                .Where(a => a.UserId == userId && !a.IsFinally)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(CIModel.ApplicationUser.PostalCode)||
                string.IsNullOrEmpty(CIModel.ApplicationUser.Address))
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return RedirectToPage("ConfirmInformation", new { Id = factor.FactorId });

            }
            else
            {
                switch (CIModel.SelectedPaymentType)
                {
                    case "پرداخت در محل":
                        currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;
                        currentUser.Address = CIModel.ApplicationUser.PostalCode;
                        currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;

                        factor.Post_Type = CIModel.SelectedPostType;
                        factor.Payment_Type = CIModel.SelectedPaymentType;
                        factor.Description = CIModel.Description;
                        factor.DeliverState = 1;

                        factor.WillDeliver_Date = DateTime.Now.AddDays(10);
                        factor.Send_Date = DateTime.Now.AddDays(2);

                        factor.PurchaseNumber = (new Random().Next(0, 500)).ToString(); ;
                        factor.IsFinally = true;

                        _db.Update(currentUser);
                        _db.Update(factor);
                        await _db.SaveChangesAsync();
                        return RedirectToPage("PaymentInfo", new { Id = factor.FactorId });

                    case "پرداخت اینترنتی":
                        currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;
                        currentUser.Address = CIModel.ApplicationUser.PostalCode;
                        currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;

                        factor.Post_Type = CIModel.SelectedPostType;
                        factor.Payment_Type = CIModel.SelectedPaymentType;
                        factor.Description = CIModel.Description;
                        factor.DeliverState = 1;

                        factor.WillDeliver_Date = DateTime.Now.AddDays(10);
                        factor.Send_Date = DateTime.Now.AddDays(2);

                        factor.PurchaseNumber = (new Random().Next(0, 500)).ToString(); ;
                        factor.IsFinally = true;


                        _db.Update(currentUser);
                        _db.Update(factor);
                        await _db.SaveChangesAsync();
                        return RedirectToPage("PaymentInfo", new { Id = factor.FactorId });

                    case "پرداخت با کیف پول":
                        double FullFactorPrice = factor.FactorDetails.Sum(a => a.Price);
                        if (currentUser.WalletAmount >= FullFactorPrice)
                        {
                            currentUser.WalletAmount -= FullFactorPrice;

                            currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;
                            currentUser.Address = CIModel.ApplicationUser.PostalCode;
                            currentUser.PostalCode = CIModel.ApplicationUser.PostalCode;

                            factor.Post_Type = CIModel.SelectedPostType;
                            factor.Payment_Type = CIModel.SelectedPaymentType;
                            factor.Description = CIModel.Description;
                            factor.DeliverState = 1;

                            factor.WillDeliver_Date = DateTime.Now.AddDays(SD.CountOfDaysPackageWillDeliver);
                            factor.Send_Date = DateTime.Now.AddDays(1);

                            factor.PurchaseNumber = (new Random().Next(0, 500)).ToString(); ;
                            factor.IsFinally = true;

                            _db.Update(currentUser);
                            _db.Update(factor);
                            await _db.SaveChangesAsync();
                            return RedirectToPage("PaymentInfo", new { Id = factor.FactorId });
                        }
                        else
                        {
                            #region Notif
                            TempData["State"] = Notifs.Error;
                            TempData["Msg"] = "موجودی کیف پول شما کافی نمی‌باشد";
                            #endregion
                            return RedirectToPage("PaymentInfo", new { Id = factor.FactorId });
                        }

                    default:
                        #region Notif
                        TempData["State"] = Notifs.Error;
                        TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                        #endregion
                        return NotFound();
                }
                

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
