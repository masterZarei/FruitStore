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
using FS.Models.ViewModels;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Admin.Orders
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public HistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }


        [BindProperty]
        public List<AdminIndexOrderVM> Model { get; set; }
        public async Task<IActionResult> OnGet()
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Model = new List<AdminIndexOrderVM>();
            var Order = _db.Factors
                 .Where(a => a.isCompleted == true)
                 .Include(a => a.User)
                 .Include(a => a.FactorDetails)
                 .ThenInclude(a => a.Product).ToList();


            foreach (var item in Order)
            {
                Model.Add(new AdminIndexOrderVM
                {
                    ID = item.User.Id,
                    FullName = $"{item.User.Name} {item.User.LastName}",
                    OrderCount = Order.Count,
                    OrderId = item.FactorId,
                    ReadyToDeliver = item.isReadyToDeliver
                }

                );
            }

            return Page();
        }
    }
}
