using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FS.DataAccess;
using FS.Models.Models;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    public class DeleteModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public DeleteModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ContactWays ContactWays { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ContactWays = await _context.ContactWays.FirstOrDefaultAsync(m => m.Id == id);

            if (ContactWays == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ContactWays = await _context.ContactWays.FindAsync(id);

            if (ContactWays != null)
            {
                _context.ContactWays.Remove(ContactWays);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
