using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages.Admin.Discounts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Discount> Discount { get; set; }

        public async Task OnGetAsync()
        {
            Discount = await _context.Discounts
                .OrderByDescending(a=>a.CreateDate)
                .ToListAsync();
        }
        public async Task<IActionResult> OnPostRemoveAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }

            var dc = await _context.Discounts.FindAsync(id);

            if (dc == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return Page();
            }
            _context.Discounts.Remove(dc);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion

            return RedirectToPage("./Index");
        }
    }
}
