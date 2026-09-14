using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.AppServices;

namespace FS.FruitStore.Pages
{
    public class Product_DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public Product_DetailsModel(ApplicationDbContext db, IUserService userService, IOrderService orderService)
        {
            _db = db;
            _userService = userService;
            _orderService = orderService;
        }
        public string ProductUnit;


        [BindProperty]
        public Product Product { get; set; }
        public List<Product> SuggestProduct { get; set; }
        [BindProperty]
        public List<Category> ProdCats { get; set; }

        #region Comments
        [BindProperty]
        public Comment Comments { get; set; }
        [BindProperty]
        public List<Comment> lstComments { get; set; }
        #endregion
        #region Icons
        //لیست رو پر میکنه
        public SelectList Unit { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedUnit { get; set; }

        #endregion

        [BindProperty]
        public List<Rating> Rating { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {

            if (id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            Product = await _db.Products
                .Where(a => a.ProductId == id)
                .OrderByDescending(a=>a.CreateDate)
                .FirstOrDefaultAsync();

            SuggestProduct = await _db.Products
                .OrderByDescending(a=>a.CreateDate)
                .Take(4)
                .ToListAsync();

            if (Product == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }


            //دسته بندیهای محصول
            ProdCats = await (from a in _db.Categories
                              join b in _db.CategoryToProducts on a.Id equals b.CategoryId
                              where b.ProductId == Product.ProductId
                              select a).ToListAsync();

            //Getting unit

            var productUnits = await _db.UnitToProducts
                .Include(a=>a.Unit)
                .Where(a => a.ProductId == Product.ProductId)
                .Select(a=>a.Unit)
                .ToListAsync();

            if (productUnits != null)
                Unit = new SelectList(productUnits,"Name", "Name");

            //کامنتهای محصول
            lstComments = await _db.Comments
                .Include(a=>a.User)
                .Where(a => a.Product_Id == id && a.isVerified == true)
                .OrderByDescending(a=>a.CreateDate)
                .ToListAsync();


            Rating = await _db.Ratings
                .Where(a => a.ProductId == Product.ProductId)
                .ToListAsync();

            return Page();

        }
        public async Task<IActionResult> OnPost(int ProductId)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Redirect("/Identity/Account/Login");
            }
            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            var result = await _orderService.AddToCartAsync(userId, ProductId, Product.Count, SelectedUnit);

            if (result.Success)
            {
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
            }
            else
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = result.Message;
                #endregion
            }
            return RedirectToPage("Product-Details", new { Id = ProductId });
        }
        public async Task<IActionResult> OnPostAddCmt(int ProductId)
        {
            if (ProductId == 0 || Comments.Text == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("Product-Details", new { id = ProductId });
            }
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Redirect("/Identity/Account/Login");
            }
            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            var currentProduct = await _db.Products
                .Where(a => a.ProductId == ProductId)
                .FirstOrDefaultAsync();

            if (currentProduct == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }

            if (Comments == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }


            var cmt = new Comment();

            cmt.Text = Comments.Text;
            cmt.User_Id = userId;
            cmt.Product_Id = currentProduct.ProductId;

            _db.Add(cmt);
            await _db.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Product-Details", new { id = ProductId });

        }
        public async Task<IActionResult> OnPostRating(byte Rate, int ProductId)
        {
            if (Rate < 0 || Rate > 5)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.ERRORHAPPEDNED;
                #endregion
                return RedirectToPage("Product-Details", new { id = ProductId });
            }

            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Redirect("/Identity/Account/Login");
            }
            var userId = _userService.GetByUsername(User.Identity.Name).Id;


            var checkIfRatedAlready = await _db.Ratings
                .Where(a => a.UserId == userId && a.ProductId == Product.ProductId)
                .FirstOrDefaultAsync();

            if (checkIfRatedAlready != null)
            {
                checkIfRatedAlready.Rate = Rate;
                _db.Update(checkIfRatedAlready);
                _db.SaveChanges();
                #region Notif
                TempData["State"] = Notifs.Success;
                TempData["Msg"] = Notifs.SUCCEEDED;
                #endregion
                return RedirectToPage("Product-Details", new { id = ProductId });
            }

            var newRating = new Rating()
            {
                ProductId = Product.ProductId,
                UserId = userId,
                Rate = Rate
            };
            _db.Add(newRating);
            await _db.SaveChangesAsync();

            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Product-Details", new { id = ProductId });


        }
    }
}