using Utilities.Roles;
using FS.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Models.ViewModels;
using Utilities.Convertors;
using FS.Models.Models;

namespace FS.FruitStore.Pages.Admin.Orders
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        [BindProperty]
        public List<AdminIndexOrderVM> Model { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {


            Model = new List<AdminIndexOrderVM>();
            var Order = await _db.Factors
                 .Where(a => a.isCompleted == false)
                 .Include(a => a.User)
                 .Include(a => a.FactorDetails)
                 .ThenInclude(a => a.Product).ToListAsync();

            foreach (var item in Order)
            {
                Model.Add(new AdminIndexOrderVM
                {
                    ID = item.User.Id,
                    FullName = $"{item.User.Name} {item.User.LastName}",
                    OrderCount = Order.Count,
                    OrderId = item.FactorId,
                    DeliverState = item.DeliverState,
                    OrderCreateDate = item.CreateDate
                });
            }

            return Page();
        }
        //ReadyToSend
        public async Task<IActionResult> OnPostReadyToSendAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
            return RedirectToPage("Index");
            }

            var selectedOrder = await _db.Factors
                 .Where(a => a.FactorId == Id)
                 .FirstOrDefaultAsync();

            if (selectedOrder == null || selectedOrder.DeliverState > 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("Index");
            }

            selectedOrder.DeliverState = 2;
            _db.Update(selectedOrder);
            await _db.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "با موفقیت انجام شد";
            #endregion

            return RedirectToPage("Index");

        }
        public async Task<IActionResult> OnPostOrderCompletedAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("Index");
            }

            var selectedOrder = await _db.Factors
                 .Where(a => a.FactorId == Id)
                 .FirstOrDefaultAsync();

            if (selectedOrder == null || selectedOrder.isCompleted == true)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
            return RedirectToPage("Index");
            }

            selectedOrder.isCompleted = true;
            selectedOrder.DeliverState = 3;

            _db.Update(selectedOrder);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "با موفقیت انجام شد";
            #endregion
            return RedirectToPage("Index");

        }
        

    }
}
