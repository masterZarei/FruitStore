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

        public GetUserInfo getInfo { get; set; }
        public async Task<IActionResult> OnGet(int Id)
        {
            if (Id == 0)
                return NotFound();

            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_db);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            Comments = _db.Comments.Where(a => a.Product_Id == Id && string.IsNullOrEmpty(a.Answer)).Include(a => a.Product).ToList();
            if (Comments.Count < 1)
                return RedirectToPage("Index");

            if (Comments == null)
                return NotFound();

            getInfo = new GetUserInfo(_db);

            return Page();

        }
    }
}
