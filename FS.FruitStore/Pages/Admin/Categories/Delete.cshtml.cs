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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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


            Category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "مشکلی رخ داد!";
                #endregion
                return Page();
            }

            Category = await _context.Categories.FindAsync(id);

            if (Category != null)
            {
                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "دسته بندی با موفقیت حذف شد.";
            #endregion
            return RedirectToPage("./Index");
        }
    }
}
