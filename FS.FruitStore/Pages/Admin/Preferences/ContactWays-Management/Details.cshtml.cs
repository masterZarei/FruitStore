using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ContactWays ContactWays { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();
            

            ContactWays = await _context.ContactWays.FirstOrDefaultAsync(m => m.Id == id);

            if (ContactWays == null)
                return NotFound();

            return Page();
        }
    }
}
