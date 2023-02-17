using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Categories
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {


            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "دسته بندی پیدا نشد!";
                #endregion
                return RedirectToPage("/NotFound");
            }


            Category = await _context
                .Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "دسته بندی پیدا نشد!";
                #endregion
                return RedirectToPage("/NotFound");
            }

            return Page();
        }
    }
}
