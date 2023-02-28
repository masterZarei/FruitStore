using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Users
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

        public async Task<IActionResult> OnGetAsync(string userId)
        {

            if (userId.Trim().Length == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }


            ApplicationUser = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (ApplicationUser == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            //نقش کاربر عادی رو بگیر
            var userRoles = _userManager.GetRolesAsync(new IdentityUser() { Id = ApplicationUser.Id }).Result; //(ClaimsIdentity)User.Identity;
            //لیست رو با اطلاعات نقش ها پر کن و برای کاربر فعلی نقش خودش رو به صورت انخاب شده قرار بده
            Roles = new SelectList(_rolemanager.Roles, "Name", "Name", userRoles.First());

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return RedirectToPage("Index", new { userId = ApplicationUser.Id });
            }

            var userInDb = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Id == ApplicationUser.Id);

            if (userInDb == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            #region Mapping
            userInDb.Name = ApplicationUser.Name;
            userInDb.LastName = ApplicationUser.LastName;
            userInDb.PhoneNumber = ApplicationUser.PhoneNumber;
            userInDb.UserName = ApplicationUser.PhoneNumber;
            userInDb.Email = ApplicationUser.Email;
            userInDb.Address = ApplicationUser.Address;
            userInDb.NormalizedUserName = ApplicationUser.PhoneNumber;
            #endregion
            

            //نقش کاربرو بگیر
            var userRoles = _userManager.GetRolesAsync(new IdentityUser() { Id = ApplicationUser.Id }).Result; //(ClaimsIdentity)User.Identity;

            //اگه عوض شده بود
            if (SelectedRole != userRoles.FirstOrDefault())
            {
                //نقش قبلیش رو پاک کن
                await _userManager.RemoveFromRoleAsync(new IdentityUser() { Id = ApplicationUser.Id }, userRoles.First());
                //نقش انتخاب شده رو بهش بده
                await _userManager.AddToRoleAsync(new IdentityUser() { Id = ApplicationUser.Id }, SelectedRole);
               
            }

          
            _context.Update(userInDb);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}
