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

namespace FS.FruitStore.Pages.Admin.Wallet
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public HistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<WalletHistory> WalletHistory { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name);
            WalletHistory = await _db.WalletHistories
                .Where(a=>a.UserId == userId.Id)
                .ToListAsync();

            return Page();
        }
    }
}
