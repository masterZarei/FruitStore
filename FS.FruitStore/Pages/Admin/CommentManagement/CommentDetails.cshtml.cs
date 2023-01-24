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
        public async Task<IActionResult> OnGet(int Id)
        {
            //#region isDisabled?
            //Methods mtd = new Methods(_db);
            //int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            //if (isAuthorized == 1)
            //    return Redirect("/Identity/Account/AccessDenied");
            //#endregion

            if (Id == 0)
                return NotFound();

            Comments = _db.Comments.Where(a => a.Id == Id).Include(a => a.Product).FirstOrDefault();
            if (Comments == null)
                return NotFound();

            return Page();

        }

        public async Task<IActionResult> OnPost(int Id)
        {
            //   Methods mtd = new Methods(_db);

            var cmt = await _db.Comments.Where(a => a.Id == Id).FirstOrDefaultAsync();
            if (cmt == null)
                return NotFound();

            cmt.isVerified = true;
            cmt.Answer = Comments.Answer;
            cmt.Responder = "فروشنده";


            _db.Update(cmt);
            await _db.SaveChangesAsync();

            return RedirectToPage("ProductsComments", new { Id = cmt.Product_Id });

        }
        public async Task<IActionResult> OnPostRemoveCmt(int Id)
        {
            if (Id == 0)
                return NotFound();

            var cmt = _db.Comments.Where(a => a.Id == Id).FirstOrDefault();
            if (cmt == null)
                return NotFound();

            _db.Remove(cmt);
            await _db.SaveChangesAsync();

            return RedirectToPage("ProductsComments", new { Id = cmt.Product_Id });

        }
    }
}
