using FS.DataAccess;
using FS.Models.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

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
        public decimal Amount { get; set; }

        public async Task<IActionResult> OnGet(string Id)
        {
            if (Id.Equals(null))
                return NotFound();

            ApplicationUser = _db.Users.Where(i => i.Id == Id).FirstOrDefault();
            return Page();
            
        }
        public async Task<IActionResult> OnPost()
        {
            if (Amount < 1000)
                return BadRequest();

            var currentUser = await _db.Users.FindAsync(ApplicationUser.Id);
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

            return RedirectToPage("Index");
        }
    }
}
