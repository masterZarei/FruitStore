using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using Utilities.Roles;
using FS.DataAccess;
using FS.Models.ViewModels;
using FS.Models.Paging;

namespace FS.FruitStore.Pages.Admin.Products
{
    [Authorize(Roles =SD.AdminEndUser)]
    public class verifyProductsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public verifyProductsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ProductsListViewModel ProductsListViewModel { get; set; }


        public async Task<IActionResult> OnGet(int pageId = 1, string searchName = null)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return NotFound();

            var userId = claim.Value;

            //#region isDisabled?
            //Methods mtd = new Methods(_db);
            //int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            //if (isAuthorized == 1)
            //    return Redirect("/Identity/Account/AccessDenied");
            //#endregion

            ProductsListViewModel = new ProductsListViewModel
            {

                Products = await _db.Products.Where(q=>q.isVerified==false).ToListAsync(),

            };


            //Filter

            StringBuilder param = new StringBuilder();
            param.Append(@"/Products/verifyProducts?pageId=:");

            param.Append("&searchName=");
            if (searchName != null)
                param.Append(searchName);




            if (searchName != null)
            {
                ProductsListViewModel.Products = _db.Products.Where(q => q.isVerified == false && q.Name.Contains(searchName)).ToList();
            }

            //Pages

            var count = ProductsListViewModel.Products.Count;
            ProductsListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageId,
                ItemPerPage = SD.PagingUserCount,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            ProductsListViewModel.Products = ProductsListViewModel.Products.OrderByDescending(a => a.CreateDate)
                .Skip((pageId - 1) * SD.PagingUserCount)
                .Take(SD.PagingUserCount).ToList();



            return Page();
        }
    }
}
