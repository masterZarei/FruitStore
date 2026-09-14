using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.Pages.Panel.PersonalInfo
{
    [Authorize]
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public IndexModel(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [BindProperty]
        public User ApplicationUser { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            ApplicationUser = await _db
                .Users
                .FirstOrDefaultAsync(a => a.Id == userId);

            if (ApplicationUser == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            #region isDisabled?
            if (_userService.IsDisabled(User.Identity.Name))
                return Redirect("/Identity/Account/AccessDenied");
            #endregion


            return Page();


        }
        public string retString(string input)
        {
            if (string.IsNullOrEmpty(input) || input == "ns")
                return "<span class='text-secondary'> مقداری وارد نشده است </span>";
            else
                return input;

        }
    }
}