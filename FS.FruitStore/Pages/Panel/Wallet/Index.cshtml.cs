using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.Pages.Panel.Wallet
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        public IndexModel(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [BindProperty]
        public UserWalletVM UserWalletVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var CurrentUser = _userService.GetByUsername(User.Identity.Name);
            UserWalletVM = new UserWalletVM()
            {
                ApplicationUser = CurrentUser,
                WalletHistory = await _db.WalletHistories.Where(a=>a.UserId == CurrentUser.Id).ToListAsync()
            };

            return Page();
        }
    }
}