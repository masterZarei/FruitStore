using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Wallet
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
                WalletHistory = _db.WalletHistories.Where(a=>a.UserId == CurrentUser.Id).ToList()
            };

            return Page();
        }
    }
}
