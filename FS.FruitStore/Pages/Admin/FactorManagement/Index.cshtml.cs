using FS.DataAccess;
using FS.Models.Models;
using Utilities.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Convertors;
using Microsoft.AspNetCore.Authorization;

namespace Mahshop.Pages.FactorManagement
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Factor> Factors { get; set; }
        public async Task<IActionResult> OnGet()
        {


            Factors = await _db.Factors
                .Include(a=>a.FactorDetails)
                .Where(a => a.IsFinally == true && a.FactorDetails.Count >= 1)
                .ToListAsync();

            return Page();

          
        }
    }
}
