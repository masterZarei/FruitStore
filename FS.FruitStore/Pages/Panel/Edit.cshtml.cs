using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages.Panel
{
    [Authorize]

    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public User Users { get; set; }

        public async Task<IActionResult> OnGetAsync(string userID)
        {


            if (userID == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }
            Users = await _db
                .Users
                .Where(a => a.Id == userID)
                .FirstOrDefaultAsync();
            

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
            var userInDb = await _db
                .Users
                .FirstOrDefaultAsync(u => u.Id == Users.Id);

            if (userInDb == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            userInDb.Name = Users.Name;
            userInDb.LastName = Users.LastName;
            userInDb.PhoneNumber = Users.PhoneNumber;
            userInDb.UserName = Users.PhoneNumber;
            userInDb.Email = Users.Email;
            userInDb.Address = Users.Address;
            userInDb.City = Users.City;
            userInDb.PostalCode = Users.PostalCode;
            userInDb.State = Users.State;
            userInDb.CartNumber = Users.CartNumber;

            _db.Users.Update(userInDb);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
    }
}
