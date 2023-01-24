using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    public class IndexModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public IndexModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ContactWays> ContactWays { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ContactWays = await _context.ContactWays.ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostRemove(int Id)
        {
            if (Id == 0)
                return NotFound();
            var item = _context.ContactWays.Where(a => a.Id == Id).FirstOrDefault();
            if (item == null)
                return Page();


            _context.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}