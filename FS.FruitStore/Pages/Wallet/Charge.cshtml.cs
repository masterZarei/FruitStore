using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Wallet
{
    [Authorize]
    public class ChargeModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ChargeModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public User ApplicationUser { get; set; }

        [BindProperty]
        public double Amount { get; set; }

        public async Task<IActionResult> OnGet(string Id)
        {
            if (Id.Equals(null))
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }

            ApplicationUser = await _db.Users
                .Where(i => i.Id == Id)
                .FirstOrDefaultAsync();

            return Page();
            
        }
        public async Task<IActionResult> OnPost()
        {
            if (Amount < 1000)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "لطفا مبلغی بالاتر از هزارتومان وارد کنید";
                #endregion
                return Page();
            }

            var currentUser = await _db.Users
                .FindAsync(ApplicationUser.Id);

            currentUser.WalletAmount += Amount;

            var newTransactionHistory = new WalletHistory()
            {
                NewWalletAmount = (currentUser.WalletAmount += Amount),
                State = true,
                TrackingCode = new Random().Next(0, 1024),
                UserId = currentUser.Id,
                TransactionAmount = Amount
            };

            _db.Add(newTransactionHistory);
            _db.Update(currentUser);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}
