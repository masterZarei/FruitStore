using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FS.Models.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Utilities.Convertors;
using FS.DataAccess;

namespace FS.FruitStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(Input.PhoneNumber, Input.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    #region isDisabled?
                    GetUserInfo mtd = new GetUserInfo(_db);
                    int isAuthorized = mtd.AuthorizeUser(Input.PhoneNumber);
                    if (isAuthorized == 1)
                        return Redirect("/Identity/Account/AccessDenied");
                    #endregion

                    #region Notif
                    TempData["State"] = Notifs.Success;
                    TempData["Msg"] = "ورود با موفقیت انجام شد!";
                    #endregion
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "ورود نامعتبر!";
                    #endregion
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
