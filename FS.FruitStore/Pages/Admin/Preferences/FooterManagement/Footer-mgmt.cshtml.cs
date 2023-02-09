using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Preferences.FooterManagement
{
    [Authorize(SD.AdminEndUser)]
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

        public async Task<IActionResult> OnGet()
        {
            Footer = _db.Footers.FirstOrDefault();
            //Categories
            FooterCWs = (from a in _db.ContactWays
                        join b in _db.ContactWaysToFooters on a.Id equals b.ContactWaysId
                        select a).ToList();


            ContactWays = (from a in _db.ContactWays
                        where !FooterCWs.Contains(a)
                        select a).ToList();

            if (ContactWays != null)
            {
                CWs = new SelectList(ContactWays,"Id", "Name");
            }

            if (Footer == null)
            {
                _db.Add(
                    new Footer
                    {
                        TrustSymbol = "",
                        TrustSymbol2 = "",
                        Description = ""
                    }
                    );
                _db.SaveChanges();
            }

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAddCat()
        {
            if (SelectedCwId > 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return NotFound();
            }
            /////////////////TODO: Do the Notif part
            var findCW = _db.ContactWays.Where(a => a.Id == SelectedCwId).FirstOrDefault();
            if (findCW == null)
                return Page();

            var isAlreadyAdded = _db.ContactWaysToFooters.Where(a=> a.ContactWaysId == findCW.Id).FirstOrDefault();
            if (isAlreadyAdded != null)
                return Page();

            ContactWaysToFooter cWf = new ContactWaysToFooter()
            {
                ContactWaysId = findCW.Id
            };


            _db.Add(cWf);
            await _db.SaveChangesAsync();

            return Page();

        }
        public async Task<IActionResult> OnPostRemoveCat(int Id)
        {
            if (Id == 0)
            {
                return Page();
            }

            var findCW = _db.ContactWays.Where(a => a.Id == SelectedCwId).FirstOrDefault();
            if (findCW == null)
                return NotFound();

            _db.Remove(findCW);
            await _db.SaveChangesAsync();

            return Page();

        }
    }
}
