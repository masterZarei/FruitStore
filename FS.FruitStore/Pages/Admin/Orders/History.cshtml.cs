using FS.DataAccess;
using FS.Models.ViewModels;
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
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public HistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }


        [BindProperty]
        public List<AdminIndexOrderVM> Model { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            Model = new List<AdminIndexOrderVM>();
            var Order = await _db.Factors
                 .Where(a => a.isCompleted == true)
                 .Include(a => a.User)
                 .Include(a => a.FactorDetails)
                 .ThenInclude(a => a.Product)
                 .OrderByDescending(a=>a.CreateDate).ToListAsync();


            foreach (var item in Order)
            {
                Model.Add(new AdminIndexOrderVM
                {
                    ID = item.User.Id,
                    FullName = $"{item.User.Name} {item.User.LastName}",
                    OrderCount = Order.Count,
                    OrderId = item.FactorId
                });
            }

            return Page();
        }
    }
}
