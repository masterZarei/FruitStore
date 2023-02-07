using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace FS.FruitStore.Pages
{
    public class Product_DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Product_DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Product Product { get; set; }
        public List<Product> SuggestProduct { get; set; }
        [BindProperty]
        public List<Category> ProdCats { get; set; }

        #region
        [BindProperty]
        public Comments Comments { get; set; }
        [BindProperty]
        public List<Comments> lstComments { get; set; }
        #endregion

        public GetUserInfo GetInfo { get; set; }
        [BindProperty]
        public List<Rating> Rating { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {

            if (id == 0)
                return NotFound();

            Product = _db.Products.Where(a => a.ProductId == id).FirstOrDefault();
            SuggestProduct = _db.Products.Take(4).ToList();

            if (Product == null)
                return NotFound();


            //دسته بندیهای محصول
            ProdCats = (from a in _db.Categories
                        join b in _db.CategoryToProducts on a.Id equals b.CategoryId
                        where b.ProductId == Product.ProductId
                        select a).ToList();
            //کامنته ای محصول
            lstComments = _db.Comments.Where(a => a.Product_Id == id && a.isVerified == true).ToList();
            //نمونه سازی کلاس به دست آوردن اطلاعات کاربر
            GetInfo = new GetUserInfo(_db);

            Rating = _db.Ratings
                .Where(a => a.ProductId == Product.ProductId)
                .ToList();

            return Page();

        }
        public async Task<IActionResult> OnPost(int ProductId)
        {
            var currentProduct = _db.Products
                .Where(a => a.ProductId == ProductId)
                .FirstOrDefault();

            if (currentProduct == null)
                return NotFound();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            var factor = _db.Factors
                .FirstOrDefault(o => o.UserId == userId && !o.IsFinally);
            //اگه فاکتور باز داشت
            if (factor != null)
            {
                var factorDetail = _db.FactorDetails
                    .FirstOrDefault(f => f.FactorId == factor.FactorId &&
                                         f.ProductId == currentProduct.ProductId);
                //اگه محصول تو سبدش بود
                if (factorDetail != null)
                {
                   

                    if (currentProduct.Count >= factorDetail.Count+Product.Count)
                        factorDetail.Count += Product.Count;
                    else
                    {
                        TempData["error"] = "لطفا به تعداد موجود محصول، به سبد خریدتان اضافه نمایید.";
                        return RedirectToPage("Product-Details", new { Id = ProductId });
                    }
                }
                // اگه نبود
                else
                {
                    if (currentProduct.Count > Product.Count)
                    {
                        _db.FactorDetails.Add(new FactorDetail()
                        {
                            FactorId = factor.FactorId,
                            ProductId = currentProduct.ProductId,
                            Price = currentProduct.Price,
                            Count = Product.Count
                        });
                    }
                    else
                    {
                        TempData["error"] = "لطفا به تعداد موجود محصول، به سبد خریدتان اضافه نمایید.";
                        return RedirectToPage("Product-Details", new { Id = ProductId });
                    }

                }
            }
            // اگه فاکتور باز نداشت
            else
            {
                if (currentProduct.Count > Product.Count)
                {
                    factor = new Factor
                    {
                        IsFinally = false,
                        UserId = userId
                    };
                    _db.Factors.Add(factor);
                    _db.SaveChanges();
                    _db.FactorDetails.Add(new FactorDetail()
                    {
                        FactorId = factor.FactorId,
                        ProductId = currentProduct.ProductId,
                        Price = currentProduct.Price,
                        Count = Product.Count
                    });

                }
                else
                {
                    TempData["error"] = "لطفا به تعداد موجود محصول، به سبد خریدتان اضافه نمایید.";
                    return RedirectToPage("Product-Details", new { Id = ProductId });
                }

            }
            TempData["success"] = "با موفقیت به سبد خرید اضافه گردید";

            await _db.SaveChangesAsync();




            return RedirectToPage("Product-Details", new { Id = ProductId });

        }
        public async Task<IActionResult> OnPostAddCmt(int ProductId)
        {
            if (ProductId == 0 || Comments.Text == null)
                return RedirectToPage("Product-Details", new { id = ProductId });


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            var currentProduct = _db.Products.Where(a => a.ProductId == ProductId).FirstOrDefault();
            if (currentProduct == null)
                return NotFound();



            if (Comments == null)
                return NotFound();


            var cmt = new Comments();

            cmt.Text = Comments.Text;
            cmt.User_Id = userId;
            cmt.Product_Id = currentProduct.ProductId;

            _db.Add(cmt);
            await _db.SaveChangesAsync();

            return RedirectToPage("Product-Details", new { id = ProductId });

        }
        public async Task<IActionResult> OnPostRating(byte Rate, int ProductId)
        {
            if (Rate < 0 || Rate > 5)
                return Page();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Redirect("/Identity/Account/Login");
            var userId = claim.Value;

            if (userId == null)
                return Redirect("/Identity/Account/Login");

            var checkIfRatedAlready = _db.Ratings
                .Where(a=>a.UserId == userId && a.ProductId == Product.ProductId)
                .FirstOrDefault();

            if (checkIfRatedAlready != null)
            {
                checkIfRatedAlready.Rate = Rate;
                _db.Update(checkIfRatedAlready);
                _db.SaveChanges();
            }

            var newRating = new Rating()
            {
                ProductId = Product.ProductId,
                UserId = userId,
                Rate = Rate
            };
            _db.Add(newRating);
            _db.SaveChanges();

            return RedirectToPage("Product-Details", new { id = ProductId });
            

        }
    }
}
