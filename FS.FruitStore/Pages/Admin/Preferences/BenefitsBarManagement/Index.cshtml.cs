using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Preferences.BenefitsBarManagement 
{ 
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<BenefitBar> BenefitBar { get;set; }

        public async Task OnGetAsync()
        {
            BenefitBar = await _context.BenefitBars.ToListAsync();
        }
        public async Task<IActionResult> OnPostRemove(int Id)
        {
            if (Id == 0)
                return NotFound();
            var item = _context.BenefitBars.Where(a => a.Id == Id).FirstOrDefault();
            if (item == null)
                return Page();


            _context.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
