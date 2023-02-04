using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.WalletManagement
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<WalletHistory> WalletHistory { get; set; }
        public async Task<IActionResult> OnGet()
        {
            WalletHistory = _db.WalletHistories
                .Include(a => a.User)
                .OrderByDescending(a => a.CreateDate)
                .ToList();

            return Page();
        }
    }
}
