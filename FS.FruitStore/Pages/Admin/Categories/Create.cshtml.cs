using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Categories
{
    [Authorize(Roles = SD.AdminEndUser)]

    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "مشکلی رخ داد!";
                #endregion
                return Page();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "دسته بندی با موفقیت ایجاد شد.";
            #endregion
            return RedirectToPage("./Index");

        }
    }
}
