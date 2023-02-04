using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public BenefitBar BenefitBar { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();


            BenefitBar = await _context.BenefitBars.FirstOrDefaultAsync(m => m.Id == id);

            if (BenefitBar == null)
                return NotFound();

            return Page();
        }
    }
}
