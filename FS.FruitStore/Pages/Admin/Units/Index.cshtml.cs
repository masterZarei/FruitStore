using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FS.DataAccess;
using FS.Models.Models;
using System.IO;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Admin.Units
{
    public class IndexModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public IndexModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Unit> Unit { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Unit = await _context.Units.ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostRemove(int Id)
        {
            if (Id == 0)
                return NotFound();
            var thisUnit = _context.Units.Where(a => a.Id == Id).FirstOrDefault();
            if (thisUnit == null)
                return Page();


            _context.Remove(thisUnit);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
