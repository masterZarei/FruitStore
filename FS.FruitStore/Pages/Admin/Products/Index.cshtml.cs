using FS.Models.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public IndexModel(FS.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; }

        public async Task OnGetAsync()
        {
            Product = await _context.Products
                .Include(p => p.User)
                .OrderByDescending(a=>a.CreateDate)
                .ToListAsync();
        }
    }
}
