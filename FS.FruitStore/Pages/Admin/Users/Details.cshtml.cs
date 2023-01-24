using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Utilities.Roles;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> rolemanager)
        {
            _context = context;
            _userManager = userManager;
            _rolemanager = rolemanager;

        }

        [BindProperty]
        public User ApplicationUser { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedRole { get; set; }
        //لیست رو پر میکنه
        public SelectList Roles { get; set; }
        //اگه کاربر در جدول فروشنده هم باشه این متغیر پر میشه
        public string isSeller = null;

        public async Task<IActionResult> OnGetAsync(string? userId)
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            if (userId.Trim().Length == 0)
                return NotFound();


            ApplicationUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (ApplicationUser == null)
                return NotFound();
            //نقش کاربر عادی رو بگیر
            var userRoles = _userManager.GetRolesAsync(new IdentityUser() { Id = ApplicationUser.Id }).Result; //(ClaimsIdentity)User.Identity;
            //لیست رو با اطلاعات نقش ها پر کن و برای کاربر فعلی نقش خودش رو به صورت انخاب شده قرار بده
            Roles = new SelectList(_rolemanager.Roles, "Name", "Name", userRoles.First());

            return Page();
        }
    }
}
