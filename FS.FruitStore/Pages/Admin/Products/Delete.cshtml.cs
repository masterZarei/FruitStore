using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.Products
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Products { get; set; }


        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            //#region isDisabled?
            //Methods mtd = new Methods(_context);
            //int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            //if (isAuthorized == 1)
            //    return Redirect("/Identity/Account/AccessDenied");
            //#endregion

            if (Id == 0)
                return NotFound();


            Products = await _context.Products.FirstOrDefaultAsync(u => u.ProductId == Id);

            if (Products == null)
                return NotFound();


            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? Id)
        {

            if (Id == 0)
            {
                return NotFound();
            }

            Products = await _context.Products.FindAsync(Id);

            if (Products == null)
                return NotFound();

            Products.isVerified = false;

            _context.Update(Products);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");

        }
    }
}
