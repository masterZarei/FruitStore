using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;

        }

        [BindProperty]
        public UsersListViewModel UsersListViewModel { get; set; }


        public async Task<IActionResult> OnGet()
        {

            UsersListViewModel = new UsersListViewModel
            {
                ApplicationUserList = await _db
                .Users
                .OrderByDescending(a => a.reg_Date)
                .ToListAsync(),

            };


            return Page();
        }
    }
}
