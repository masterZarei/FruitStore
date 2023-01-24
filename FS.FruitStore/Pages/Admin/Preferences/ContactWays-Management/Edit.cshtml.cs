using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FS.DataAccess;
using FS.Models.Models;
using Utilities;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
    public class EditModel : PageModel
    {
        private readonly FS.DataAccess.ApplicationDbContext _context;

        public EditModel(FS.DataAccess.ApplicationDbContext context)
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
                return NotFound();


            ContactWays = await _context.ContactWays.FirstOrDefaultAsync(m => m.Id == id);

            if (ContactWays == null)
                return NotFound();
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
                return Page();

            ContactWays.Icon = SelectedIcon;

            if (ContactWays.IsLink && !ContactWays.Address.Contains("https://"))
                ContactWays.Address = $"https://{ContactWays.Address}";

            _context.Update(ContactWays);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
