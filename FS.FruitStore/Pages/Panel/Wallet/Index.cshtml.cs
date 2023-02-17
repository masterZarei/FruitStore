using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Admin.Wallet
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public UserWalletVM UserWalletVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var CurrentUser = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name);
            UserWalletVM = new UserWalletVM()
            {
                ApplicationUser = CurrentUser,
                WalletHistory = await _db.WalletHistories.Where(a=>a.UserId == CurrentUser.Id).ToListAsync()
            };

            return Page();
        }
    }
}
