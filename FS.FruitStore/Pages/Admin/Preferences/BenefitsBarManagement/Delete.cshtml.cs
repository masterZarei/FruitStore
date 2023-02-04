using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    public class DeleteModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public DeleteModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BenefitBar BenefitBar { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BenefitBar = await _context.BenefitBars.FirstOrDefaultAsync(m => m.Id == id);

            if (BenefitBar == null)
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

            BenefitBar = await _context.BenefitBars.FindAsync(id);

            if (BenefitBar != null)
            {
                _context.BenefitBars.Remove(BenefitBar);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
