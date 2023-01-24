using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Panel.MyFactors
{
    [Authorize]
    public class Factor_DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Factor_DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<FactorDetail> FactorDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (Id == 0)
                return NotFound();

            FactorDetail = _db.FactorDetails.Include(a=>a.Product).Where(a => a.FactorId == Id).ToList();

            if (FactorDetail == null)
                return NotFound();

            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            return Page();
        }
    }
}
