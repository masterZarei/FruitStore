using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Panel.MyFactors
{
    [Authorize]
    public class MyFactorsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MyFactorsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Factor> Order { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name).Id;

            Order = await _db.Factors
                .Include(a=>a.User)
                .Where(a => a.UserId == userId && a.IsFinally.Equals(true))
                .ToListAsync();

            return Page();
        }
    }
}
