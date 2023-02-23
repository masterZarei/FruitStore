using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Users
{
    [BindProperties]
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> rolemanager)
        {
            _context = context;
            _userManager = userManager;
            _rolemanager = rolemanager;
        }

        public User ApplicationUser { get; set; }

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


            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string userId)
        {
            #region Delete
            //if (userId.Trim().Length == 0)
            //{
            //    return RedirectToPage("/NotFound");
            //}

            //ApplicationUser = await _context.ApplicationUsers.FindAsync(userId);

            //if (ApplicationUser != null)
            //{
            //    _context.ApplicationUsers.Remove(ApplicationUser);



            //    await _context.SaveChangesAsync();
            //}
            //return RedirectToPage("Index");
            #endregion

            if (userId.Trim().Length == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var ApUser = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            ApUser.isDisabled = ApplicationUser.isDisabled;

            if (ApplicationUser != null)
            {
                _context.Update(ApUser);

                await _context.SaveChangesAsync();
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}
