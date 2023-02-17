using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Panel
{
    [Authorize]
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public User ApplicationUser { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;

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
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
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
