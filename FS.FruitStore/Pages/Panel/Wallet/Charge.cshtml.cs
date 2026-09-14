using FS.DataAccess;
using FS.Models.Models;
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
    public class ChargeModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWalletService _walletService;
        public ChargeModel(ApplicationDbContext db, IWalletService walletService)
        {
            _db = db;
            _walletService = walletService;
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
                return RedirectToPage("/NotFound");
            }

            ApplicationUser = await _db.Users
                .Where(i => i.Id == Id)
                .FirstOrDefaultAsync();

            return Page();
            
        }
        public async Task<IActionResult> OnPost()
        {
            var result = await _walletService.ChargeAsync(ApplicationUser.Id, Amount);

            if (!result.Success)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = result.Message;
                #endregion
                return Page();
            }

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}