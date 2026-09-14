using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.Pages.Panel.Wallet
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        public HistoryModel(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        public List<WalletHistory> WalletHistory { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var userId = _userService.GetByUsername(User.Identity.Name).Id;
            WalletHistory = await _db.WalletHistories
                .Where(a=>a.UserId == userId)
                .ToListAsync();

            return Page();
        }
    }
}