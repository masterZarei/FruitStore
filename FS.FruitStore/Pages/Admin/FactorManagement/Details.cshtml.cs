using FS.DataAccess;
using FS.Models.Models;
using Utilities.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace Mahshop.Pages.FactorManagement
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Factor Factor { get; set; }
        [BindProperty]
        public GetUserInfo GetInfo { get; set; }
        public async Task<IActionResult> OnGet(int Id)
        {
            if (Id == 0)
                return NotFound();
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Factor = _db.Factors
                .Include(a=>a.FactorDetails)
                .ThenInclude(a=>a.Product)
                .ThenInclude(a=>a.User)
                .Where(a => a.FactorId == Id)
                .OrderByDescending(a=>a.CreateDate)
                .FirstOrDefault();

            if (Factor == null)
                return NotFound();
            GetInfo = new GetUserInfo(_db);

            return Page();
        }
    }
}
