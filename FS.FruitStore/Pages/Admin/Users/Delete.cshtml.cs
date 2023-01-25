using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Utilities.Roles;
using FS.DataAccess;
using Utilities.Convertors;
using FS.Models.Models;

namespace FS.FruitStore.Pages.Users
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


            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? userId)
        {
            #region Delete
            //if (userId.Trim().Length == 0)
            //{
            //    return NotFound();
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
                return NotFound();
            }
            var ApUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            ApUser.isDisabled = ApplicationUser.isDisabled;

            if (ApplicationUser != null)
            {
                _context.Update(ApUser);



                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}