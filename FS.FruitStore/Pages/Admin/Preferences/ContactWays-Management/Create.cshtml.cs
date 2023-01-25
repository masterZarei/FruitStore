﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FS.DataAccess;
using FS.Models.Models;
using Utilities;

namespace FS.FruitStore.Pages.Admin.Preferences.ContactWays_Management
{
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
        public async Task<IActionResult> OnGet()
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
                initIcons();
                return Page();
            }

            ContactWays.Icon = SelectedIcon;
            if (ContactWays.IsLink)
                ContactWays.Address = $"https://{ContactWays.Address}";

            _context.ContactWays.Add(ContactWays);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}