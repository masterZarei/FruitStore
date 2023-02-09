using FS.DataAccess;
using FS.Models.Paging;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Users
{
    [Authorize(SD.AdminEndUser)]
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


        public async Task<IActionResult> OnGet(int pageId = 1, string searchName = null, string searchPhone = null)
        {

            UsersListViewModel = new UsersListViewModel
            {
                ApplicationUserList = await _db
                .Users
                .OrderByDescending(a => a.reg_Date)
                .ToListAsync(),

            };
            //Filter

            StringBuilder param = new StringBuilder();
            param.Append(@"/User?pageId=:");

            param.Append("&searchName=");
            if (searchName != null)
                param.Append(searchName);


            param.Append("&searchPhone=");
            if (searchPhone != null)
                param.Append(searchPhone);

            if (searchName != null || searchPhone != null)
            {
                UsersListViewModel.ApplicationUserList = await _db
                    .Users
                    .Where(u => u.Name.Contains(searchName) ||
                    u.PhoneNumber.Contains(searchPhone) ||
                    u.LastName.Contains(searchName))
                    .ToListAsync();
            }

            //Pages

            var count = UsersListViewModel.ApplicationUserList.Count;
            UsersListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageId,
                ItemPerPage = SD.PagingUserCount,
                TotalItems = count,
                UrlParam = param.ToString()
            };
            UsersListViewModel.ApplicationUserList = UsersListViewModel.ApplicationUserList.OrderBy(u => u.Name)
                .Skip((pageId - 1) * SD.PagingUserCount)
                .Take(SD.PagingUserCount).ToList();



            return Page();
        }
    }
}
