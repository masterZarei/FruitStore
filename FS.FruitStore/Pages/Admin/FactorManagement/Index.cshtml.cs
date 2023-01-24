using FS.DataAccess;
using FS.Models.Models;
using Utilities.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace Mahshop.Pages.FactorManagement
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Factor> Factors { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Factors = _db.Factors
                .Include(a=>a.FactorDetails)
                .Where(a => a.IsFinally == true && a.FactorDetails.Count >= 1)
                .ToList();

            return Page();

          
        }
    }
}
