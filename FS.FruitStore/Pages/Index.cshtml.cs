using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async Task<ActionResult> OnGet()
        {
            Product = _db.Products.ToList();

            Sliders = _db.Sliders.ToList();

            BenefitBars = _db.BenefitBars.ToList();

            return Page();
        }
    }
}
