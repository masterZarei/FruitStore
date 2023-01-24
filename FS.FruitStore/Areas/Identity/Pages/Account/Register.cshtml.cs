using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FS.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using FS.DataAccess;
using Utilities.Roles;
using Utilities;
using Services.SMSService;

namespace FS.FruitStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly ISMSService _smsService;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            ISMSService smsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _roleManager = roleManager;
            _smsService = smsService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "لطفا نام خود را وارد کنید")]
            [Display(Name = "نام ")]
            [MaxLength(200)]
            public string Name { get; set; }

            [Required(ErrorMessage = "لطفا نام خانوادگی خود را وارد کنید")]
            [Display(Name = "نام خانوادگی ")]
            [MaxLength(200)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "لطفا شماره همراه خود را وارد کنید")]
            [MaxLength(11)]
            [Display(Name = "شماره تلفن")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر و دارای حروف و اعداد باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن با هم همخوانی ندارند.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserName = Input.PhoneNumber,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    LastName = Input.LastName,
                    VerificationCode = Generator.RandomNumber()
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //اگه نقش ها وجود ندارند اضافه شون کن
                    if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }
                    // به کاربر نقش ادمین بده
                    await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                    _logger.LogInformation("User created a new account with password.");

                    //کد تایید باید ارسال شود
                    string name = Input.Name;
                    string code = user.VerificationCode;
                    var message = $"{name} عزیز\n کد تایید شما: \n {code}";

                    await _smsService.SendPublicSMS("09367472136", message);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToPage("/Verification/VerifyPhonenumber", new {Id = user.Id});
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }

    
}
