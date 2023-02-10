using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Orders
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Factor> Order { get; set; }
        public User ApplicationUser { get; set; }
        public async Task<IActionResult> OnGetAsync(string Id, bool isIndex = true)
        {

            if (Id.Trim().Length == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion               
                return NotFound();
            }
            if (isIndex)
            {
                Order = await _db.Factors
               .Where(a => a.isCompleted == false &&
                       a.User.Id == Id)
               .Include(a => a.User)
               .Include(a => a.FactorDetails)
               .ThenInclude(a => a.Product)
               .ToListAsync();

                ApplicationUser = await _db.Users
                    .FindAsync(Id);
            }
            else
            {
                Order = await _db.Factors
              .Where(a => a.isCompleted == true &&
                      a.User.Id == Id)
              .Include(a => a.User)
              .Include(a => a.FactorDetails)
              .ThenInclude(a => a.Product)
              .ToListAsync();

                ApplicationUser = await _db.Users
                    .FindAsync(Id);
            }


            return Page();
        }

    }
}
