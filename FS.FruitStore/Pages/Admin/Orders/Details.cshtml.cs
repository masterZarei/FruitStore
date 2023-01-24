using Utilities.Roles;
using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Admin.Orders
{
    [Authorize(Roles =SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Factor> Order { get; set; }
        public async Task<IActionResult> OnGet(string Id,bool isIndex = true)
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            if (Id.Trim().Length == 0)
                return NotFound();
            if (isIndex)
            {
                Order = _db.Factors
               .Where(a => a.isCompleted == false &&
                       a.User.Id == Id)
               .Include(a => a.User)
               .Include(a => a.FactorDetails)
               .ThenInclude(a => a.Product)
               .ToList();
            }
            else
            {
                Order = _db.Factors
              .Where(a => a.isCompleted == true &&
                      a.User.Id == Id)
              .Include(a => a.User)
              .Include(a => a.FactorDetails)
              .ThenInclude(a => a.Product)
              .ToList();
            }
           

            return Page();
        }
        
    }
}
