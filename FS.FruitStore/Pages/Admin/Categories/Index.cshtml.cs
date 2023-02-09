using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Categories
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //#region isDisabled?
            //Methods mtd = new Methods(_context);
            //int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            //if (isAuthorized == 1)
            //    return Redirect("/Identity/Account/AccessDenied");
            //#endregion
            
            Category = await _context.Categories.ToListAsync();
            return Page();
        }
    }
}
