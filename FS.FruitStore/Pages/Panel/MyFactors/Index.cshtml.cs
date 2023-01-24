using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Panel.MyFactors
{
    [Authorize]
    public class MyFactorsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MyFactorsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Factor> Order { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");

            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            var userId = claim.Value;

            Order = _db.Factors.Where(a => a.UserId == userId && a.IsFinally.Equals(true)).ToList();

            return Page();
        }
    }
}
