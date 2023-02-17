using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.CommentManagement
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class ProductsCommentsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ProductsCommentsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<Comments> Comments { get; set; }

        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }


            Comments = await _db.Comments
                .Where(a => a.Product_Id == Id && string.IsNullOrEmpty(a.Answer))
                .Include(a => a.Product)
                .ThenInclude(a=> a.User)
                .ToListAsync();

            if (Comments.Count < 1)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "تعداد کامنت ها از 1 کمتر است";
                #endregion
                return RedirectToPage("Index");
            }

            if (Comments == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            return Page();

        }
    }
}
