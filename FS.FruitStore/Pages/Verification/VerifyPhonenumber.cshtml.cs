using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.Pages.Verification
{
    [Authorize]
    public class VerifyPhonenumberModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public VerifyPhonenumberModel(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [BindProperty]
        public User ApplicationUser { get; set; }

        [BindProperty]
        public string EnteredCode { get; set; }
        public async Task<IActionResult> OnGet(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
                ApplicationUser = await _db.Users.FindAsync(Id);
            else
            {
                var user = _userService.GetByUsername(User.Identity.Name);
                ApplicationUser = await _db.Users.FindAsync(user.Id);
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(EnteredCode))
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }
            var user = await _db.Users
                .FindAsync(ApplicationUser.Id);

            if (user.VerificationCode == EnteredCode)
                user.isVerified = true;
            else
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "کد وارد شده معتبر نمی باشد";
                #endregion
                return Page();
            }
            _db.Update(user);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return Redirect("/");
        }
    }
}