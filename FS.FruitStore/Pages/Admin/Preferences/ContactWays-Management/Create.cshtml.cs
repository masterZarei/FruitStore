using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public ContactWays ContactWays { get; set; }

        #region Icons
        //لیست رو پر میکنه
        public SelectList Icons { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedIcon { get; set; }

        #endregion
        public IActionResult OnGet()
        {
            // Icons
            initIcons();

            return Page();

        }
        void initIcons()
        {
            var AllIcons = IconStore.GetIcons;
            if (AllIcons != null)
                Icons = new SelectList(AllIcons, "Value", "Name");

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                initIcons();
                return Page();
            }

            ContactWays.Icon = SelectedIcon;
            if (ContactWays.IsLink)
                ContactWays.Address = $"https://{ContactWays.Address}";

            _context.ContactWays.Add(ContactWays);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./Index");
        }
    }
}
