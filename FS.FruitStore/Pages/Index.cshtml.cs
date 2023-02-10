using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;


        public IndexModel( ApplicationDbContext db)
        {
            
            _db = db;
        }
        public List<Product> Product  { get; set; }
        public List<Slider> Sliders  { get; set; }
        public List<BenefitBar> BenefitBars  { get; set; }
        public async Task<ActionResult> OnGetAsync()
        {
            Product = await _db.Products.ToListAsync();

            Sliders = await _db.Sliders.ToListAsync();

            BenefitBars = await _db.BenefitBars.ToListAsync();

            return Page();
        }
    }
}
