using FS.DataAccess;
using FS.Models.Models;
using FS.Models.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.FruitStore.Pages
{
    public class AllProductsModel : PageModel
    {
        private const int PageSize = 8;

        private readonly ApplicationDbContext _context;

        public AllProductsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Cat
        [BindProperty]
        public List<Category> Category { get; set; }
        //لیست رو پر میکنه
        public SelectList Cats { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedCat { get; set; }
        #endregion

        public IList<Product> Product { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(string SC, string search, int page = 1)
        {
            page = page < 1 ? 1 : page;

            Category = await (from a in _context.Categories
                              select a).ToListAsync();

            if (Category != null)
            {
                Cats = new SelectList(Category, "Name", "Name");
            }

            IQueryable<Product> query;
            if (SC != null && SC != "همه")
            {
                query = from p in _context.Products
                        join ctp in _context.CategoryToProducts on p.ProductId equals ctp.ProductId
                        where ctp.Category.Name == SC && p.isVerified
                        select p;
                SelectedCat = SC;
            }
            else if (SC == "همه")
            {
                query = _context.Products.Where(a => a.isVerified);
                SelectedCat = SC;
            }
            else if (search != null)
            {
                query = _context.Products.Where(a => a.Name.Contains(search));
            }
            else
            {
                query = _context.Products.Where(a => a.isVerified);
            }

            int totalItems = await query.CountAsync();

            Product = await query
                .OrderByDescending(a => a.CreateDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            string urlParam = $"?SC={SC ?? ""}&search={search ?? ""}";
            PagingInfo = new PagingInfo
            {
                TotalItems = totalItems,
                ItemPerPage = PageSize,
                CurrentPage = page,
                UrlParam = urlParam
            };

            return Page();

        }
        public IActionResult OnPostFilCat()
        {
            if (SelectedCat != null)
            {
                return RedirectToPage("AllProducts", new { SC = SelectedCat });
            }
            return Page();
        }


    }
}