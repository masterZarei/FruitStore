using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Verification
{
    [Authorize]
    public class VerifyPhonenumberModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public VerifyPhonenumberModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public User ApplicationUser { get; set; }

        [BindProperty]
        public string EnteredCode { get; set; }
        public async Task<IActionResult> OnGet(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
                ApplicationUser = _db.Users.Find(Id);
            else
            {
                var userId = new GetUserInfo(_db)
                                .GetInfoByUsername(User.Identity.Name);
                ApplicationUser = _db.Users.Find(userId);
            }


            return Page();
        }
        public async Task<IActionResult> OnPost()
        {


            if (string.IsNullOrEmpty(EnteredCode))

                return Page();

            var user = _db.Users.Find(ApplicationUser.Id);

            if (user.VerificationCode == EnteredCode)
                user.isVerified = true;
            else
            {
                ModelState.AddModelError("CodeNotMatched", "کد وارد شده معتبر نمی باشد!");
                return Page();
            }

            _db.Update(user);
            _db.SaveChanges();

            return Redirect("/");


        }
    }
}
