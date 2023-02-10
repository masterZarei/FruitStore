using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.CommentManagement
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CommentDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CommentDetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Comments Comments { get; set; }
        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
            return NotFound();
            }

            Comments = await _db.Comments
                .Where(a => a.Id == Id)
                .Include(a => a.Product)
                .FirstOrDefaultAsync();
            if (Comments == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
            }

            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int Id)
        {
            //   Methods mtd = new Methods(_db);

            var cmt = await _db.Comments
                .Where(a => a.Id == Id)
                .FirstOrDefaultAsync();

            if (cmt == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion

                return NotFound();
            }

            cmt.isVerified = true;
            cmt.Answer = Comments.Answer;
            cmt.Responder = "فروشنده";


            _db.Update(cmt);
            await _db.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "نظر با موفقیت پاسخ داده شد";
            #endregion
            return RedirectToPage("ProductsComments", new { Id = cmt.Product_Id });

        }
        public async Task<IActionResult> OnPostRemoveCmt(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            var cmt = await _db.Comments
                .Where(a => a.Id == Id)
                .FirstOrDefaultAsync();

            if (cmt == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            _db.Remove(cmt);
            await _db.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = "نظر با موفقیت حذف شد";
            #endregion
            return RedirectToPage("ProductsComments", new { Id = cmt.Product_Id });

        }
    }
}
