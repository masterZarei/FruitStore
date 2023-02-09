using FS.DataAccess;
using FS.Models.Paging;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Convertors;
using Utilities.Roles;

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

            var userId = new GetUserInfo(_db).GetInfoByUsername(User.Identity.Name);


            ProductsListViewModel = new ProductsListViewModel
            {

                Products = await _db
                .Products
                .Where(q=>q.isVerified==false)
                .ToListAsync(),

            };


            //Filter

            StringBuilder param = new StringBuilder();
            param.Append(@"/Products/verifyProducts?pageId=:");

            param.Append("&searchName=");
            if (searchName != null)
                param.Append(searchName);




            if (searchName != null)
            {
                ProductsListViewModel.Products = await _db
                    .Products
                    .Where(q => q.isVerified == false && q.Name.Contains(searchName))
                    .ToListAsync();
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

            ProductsListViewModel.Products = ProductsListViewModel
                .Products
                .OrderByDescending(a => a.CreateDate)
                .Skip((pageId - 1) * SD.PagingUserCount)
                .Take(SD.PagingUserCount).ToList();



            return Page();
        }
    }
}
