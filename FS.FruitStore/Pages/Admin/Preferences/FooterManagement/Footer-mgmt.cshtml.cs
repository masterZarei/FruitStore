using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.FooterManagement
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class Footer_mgmtModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Footer_mgmtModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Footer Footer { get; set; }

        #region ContactWays

        [BindProperty]
        public List<ContactWays> ContactWays { get; set; }

        [BindProperty]
        public List<ContactWays> FooterCWs { get; set; }

        //لیست رو پر میکنه
        public SelectList CWs { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public int SelectedCwId { get; set; }

        #endregion
        public IFormFile ImgUp { get; set; }
        [BindProperty]
        public IFormFile ImgUp2 { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Footer = await _db.Footers
                .FirstOrDefaultAsync();


            if (Footer == null)
            {
                _db.Add(new Footer
                {
                    TrustSymbol = "",
                    TrustSymbol2 = "",
                    Description = ""
                });
                _db.SaveChanges();

                Footer = await _db.Footers
                .FirstOrDefaultAsync();
            }
            //  Categories
            FooterCWs = await _db.ContactWays
               .Where(a => a.IsInFooter.Equals(true))
               .ToListAsync();

            ContactWays = await (from a in _db.ContactWays
                                 where !FooterCWs.Contains(a)
                                 select a).ToListAsync();

            if (ContactWays != null)
                CWs = new SelectList(ContactWays, "Id", "Name");


            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return RedirectToPage("Footer-mgmt");
            }


            if (ImgUp != null || ImgUp2 != null)
            {
                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences");
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
            }

            if (ImgUp != null)
            {
                // بررسی فایل ورودی
                if (ImageFormats.CheckFormats(Path.GetExtension(ImgUp.FileName)) == null)
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "لطفا عکس وارد کنید";
                    #endregion
                    return Page();
                }
                if (!string.IsNullOrEmpty(Footer.TrustSymbol))
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences", Footer.TrustSymbol);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }

                Footer.TrustSymbol = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences", Footer.TrustSymbol);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }
            if (ImgUp2 != null)
            {
                // بررسی فایل ورودی
                if (ImageFormats.CheckFormats(Path.GetExtension(ImgUp2.FileName)) == null)
                {
                    #region Notif
                    TempData["State"] = Notifs.Error;
                    TempData["Msg"] = "لطفا عکس وارد کنید";
                    #endregion
                    return Page();
                }
                if (!string.IsNullOrEmpty(Footer.TrustSymbol2))
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences", Footer.TrustSymbol2);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }
                Footer.TrustSymbol2 = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp2.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Preferences", Footer.TrustSymbol2);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp2.CopyTo(filestream);
                }
            }

            _db.Update(Footer);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion

            return RedirectToPage("Footer-mgmt");
        }
        public async Task<IActionResult> OnPostAddCW()
        {
            if (SelectedCwId == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var findCW = await _db.ContactWays
                .Where(a => a.Id == SelectedCwId)
                .FirstOrDefaultAsync();

            if (findCW == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return Page();
            }

            var isAlreadyAdded = _db.ContactWays
                .Where(a => a.IsInFooter.Equals(true) && a.Id == SelectedCwId)
                .FirstOrDefault();

            if (isAlreadyAdded != null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "این آیتم در حال حاضر در فوتر می باشد";
                #endregion
                return Page();
            }

            findCW.IsInFooter = true;

            _db.Update(findCW);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Footer-mgmt");

        }
        public async Task<IActionResult> OnPostRemoveCW(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return Page();
            }

            var findCW = _db.ContactWays
                .Where(a => a.Id.Equals(Id) && a.IsInFooter.Equals(true))
                .FirstOrDefault();

            if (findCW == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            findCW.IsInFooter = false;

            _db.Update(findCW);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Footer-mgmt");
        }
    }
}
