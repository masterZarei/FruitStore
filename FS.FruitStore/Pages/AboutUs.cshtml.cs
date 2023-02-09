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
    public class AboutUsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AboutUsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public AboutUs AboutUs { get; set; }

        [BindProperty]
        public List<ContactWays> ContactWays { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            AboutUs = await _db.AboutUs.FirstOrDefaultAsync();

            if (AboutUs == null)
            {
                var initAboutUs = new AboutUs()
                {
                    Img = "",
                    Text = ""
                };
                _db.Add(initAboutUs);
                _db.SaveChanges();
                AboutUs = await _db.AboutUs.FirstOrDefaultAsync();
            }
            ContactWays = await _db.ContactWays
                .OrderByDescending(a=>a.CreateDate)
                .ToListAsync();


            return Page();
        }
    }
}
