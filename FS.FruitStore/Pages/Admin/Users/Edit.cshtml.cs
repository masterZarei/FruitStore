using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Utilities.Roles;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> rolemanager)
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


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == ApplicationUser.Id);

            if (userInDb == null)
                return NotFound();

            userInDb.Name = ApplicationUser.Name;
            userInDb.LastName = ApplicationUser.LastName;
            userInDb.PhoneNumber = ApplicationUser.PhoneNumber;
            userInDb.UserName = ApplicationUser.PhoneNumber;
            userInDb.Email = ApplicationUser.Email;
            userInDb.Address = ApplicationUser.Address;
            userInDb.NormalizedUserName = ApplicationUser.PhoneNumber;
            

            //نقش کاربرو بگیر
            var userRoles = _userManager.GetRolesAsync(new IdentityUser() { Id = ApplicationUser.Id }).Result; //(ClaimsIdentity)User.Identity;

            //اگه عوض شده بود
            if (SelectedRole != userRoles.FirstOrDefault())
            {
                //نقش قبلیش رو پاک کن
                await _userManager.RemoveFromRoleAsync(new IdentityUser() { Id = ApplicationUser.Id }, userRoles.First());
                //نقش انتخاب شده رو بهش بده
                await _userManager.AddToRoleAsync(new IdentityUser() { Id = ApplicationUser.Id }, SelectedRole);
                //تو جدول فروشندگان نیست؟
               
            }

          
            _context.Update(userInDb);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
