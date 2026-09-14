using FS.DataAccess;
using FS.Models.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.AppServices;
using Utilities;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Payments
{
    [Authorize]
    public class ConfirmInformationModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public ConfirmInformationModel(ApplicationDbContext db, IUserService userService, IOrderService orderService)
        {
            _db = db;
            _userService = userService;
            _orderService = orderService;
        }
        [BindProperty]
        public ConfirmInformationVM CIModel { get; set; }



        public async Task<IActionResult> OnGet(int Id)
        {
            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion

                return RedirectToPage("/NotFound");
            }

            var factor = await _db.Factors
                .Where(a => a.FactorId == Id &&
                a.UserId == userId && !a.IsFinally &&
                a.FactorDetails.Count >= 1)
                .FirstOrDefaultAsync();


            CIModel = new ConfirmInformationVM()
            {
                ApplicationUser = await _db.Users
                .Where(a => a.Id == userId)
                .FirstOrDefaultAsync()
            };
            var AllPaymentTypes = PayWays.GetWays;

               var check = _db.Logos.FirstOrDefault();
                bool adminConsent;
            if (check != null)
                adminConsent = check.DeliverAtTheSameDate;
            else
                adminConsent = false;

            var DeliverDates = new List<Dictionary>();
            int i = adminConsent ? 0 : 1;
            for ( ; i < 5; i++)
            {
                DeliverDates.Add(new Dictionary { Name = $"{DateTime.Now.AddDays(i).ToShamsi()}", Value = $"{DateTime.Now.AddDays(i).ToShamsi()}" });
            }
            var DeliverTime = new List<Dictionary>()
            {
                new Dictionary {Name = "صبح تا ظهر 9 - 12", Value = "صبح تا ظهر 9 - 12"},
                new Dictionary {Name = "ظهر تا شب 12 - 22", Value = "ظهر تا شب 12 - 22"},
            };
            CIModel.DeliverTime = new SelectList(DeliverTime, "Value", "Name");
            CIModel.DeliverDate = new SelectList(DeliverDates, "Value", "Name");
            CIModel.PaymentTypes = new SelectList(AllPaymentTypes, "Value", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            var factor = await _orderService.GetOpenFactorAsync(userId);

            if (factor == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            if (string.IsNullOrEmpty(CIModel.ApplicationUser.PostalCode) ||
                string.IsNullOrEmpty(CIModel.ApplicationUser.Address))
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return RedirectToPage("ConfirmInformation", new { Id = factor.FactorId });
            }

            var result = await _orderService.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = userId,
                PaymentType = CIModel.SelectedPaymentType,
                PostalCode = CIModel.ApplicationUser.PostalCode,
                Address = CIModel.ApplicationUser.Address,
                Description = CIModel.Description,
                DeliverDate = CIModel.SelectedDeliverDate,
                DeliverTime = CIModel.SelectedDeliverTime
            });

            switch (result.Status)
            {
                case FinalizeStatus.Success:
                    return RedirectToPage("PaymentInfo", new { Id = result.FactorId });

                case FinalizeStatus.InsufficientWallet:
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "موجودی کیف پول شما کافی نمی‌باشد";
                    #endregion
                    return RedirectToPage("/");

                case FinalizeStatus.InvalidAddress:
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                    #endregion
                    return RedirectToPage("ConfirmInformation", new { Id = factor.FactorId });

                case FinalizeStatus.EmptyCart:
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.NOTFOUND;
                    #endregion
                    return RedirectToPage("/NotFound");

                default:
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                    #endregion
                    return RedirectToPage("/NotFound");
            }
        }
    }
}