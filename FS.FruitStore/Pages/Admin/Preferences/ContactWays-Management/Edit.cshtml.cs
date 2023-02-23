using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }


            ContactWays = await _context
                .ContactWays
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ContactWays == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var currentIcon = ContactWays.Icon;

            // Icons
            var AllIcons = IconStore.GetIcons;
            if (AllIcons != null)
            {
                Icons = new SelectList(AllIcons, "Value", "Name", currentIcon);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }

            ContactWays.Icon = SelectedIcon;

            if (ContactWays.IsLink && !ContactWays.Address.Contains("https://"))
                ContactWays.Address = $"https://{ContactWays.Address}";

            if (!ContactWays.IsLink && ContactWays.Address.Contains("https://"))
                ContactWays.Address.Replace("https://","");

            _context.Update(ContactWays);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("./Index");
        }

    }
}
