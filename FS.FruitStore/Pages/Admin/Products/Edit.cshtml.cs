using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities.Roles;
using FS.DataAccess;
using FS.Models.Models;
using Utilities.Convertors;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace FS.FruitStore.Pages.Admin.Products
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Product Product { get; set; }

        #region Category
        [BindProperty]
        public List<Category> Category { get; set; }

        [BindProperty]
        public List<Category> ProdCats { get; set; }

        //لیست رو پر میکنه
        public SelectList Cats { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedCat { get; set; }

        #endregion

        #region Unit
        //لیست رو پر میکنه
        public SelectList Units { get; set; }

        [BindProperty]
        //آیتم انتخابی رو نگه میداره
        public string SelectedUnit { get; set; }

        public string CurrentUnit { get; set; }
        #endregion

        [BindProperty]
        public IFormFile ImgUp { get; set; }

        [BindProperty]
        public IFormFile ImgUp1 { get; set; }

        public async Task<IActionResult> OnGet(int? Id)
        {
            #region isDisabled?
            GetUserInfo mtd = new GetUserInfo(_context);
            int isAuthorized = mtd.AuthorizeUser(User.Identity.Name);
            if (isAuthorized == 1)
                return Redirect("/Identity/Account/AccessDenied");
            #endregion

            if (Id == 0)
                return NotFound();

            Product = _context.Products.Where(c => c.ProductId == Id).FirstOrDefault();

            if (Product == null)
            {
                return NotFound();
            }

            var checkProductUnit = new GetProductInfo(_context).GetUnit(Product.ProductId);

            if (checkProductUnit != null)
                CurrentUnit = new GetProductInfo(_context).GetUnit(Product.ProductId).Name;


            //Categories
            ProdCats = (from a in _context.Categories
                        join b in _context.CategoryToProducts on a.Id equals b.CategoryId
                        where b.ProductId == Product.ProductId
                        select a).ToList();


            Category = (from a in _context.Categories
                        where !ProdCats.Contains(a)
                        select a).ToList();

            if (Category != null)
            {
                Cats = new SelectList(Category, "Name", "Name");
            }

            // Units
            var AllUnits = _context.Units.ToList();
            if (AllUnits != null)
            {
                Units = new SelectList(AllUnits, "Name", "Name",CurrentUnit);
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();



            if (ImgUp != null)
            {
                if (!string.IsNullOrEmpty(Product.ProductPic2))
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages", Product.ProductPic);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);
                }

                Product.ProductPic = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages", Product.ProductPic);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp.CopyTo(filestream);
                }
            }
            if (ImgUp1 != null)
            {
                if (!string.IsNullOrEmpty(Product.ProductPic2))
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages", Product.ProductPic2);
                    if (System.IO.File.Exists(deletePath))
                        System.IO.File.Delete(deletePath);


                }
                Product.ProductPic2 = Guid.NewGuid().ToString() + Path.GetExtension(ImgUp1.FileName);
                string savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages", Product.ProductPic2);
                using (var filestream = new FileStream(savepath, FileMode.Create))
                {
                    ImgUp1.CopyTo(filestream);
                }
            }

            var currentUnit = new GetProductInfo(_context).GetUnit(Product.ProductId);

            if (currentUnit != null)
            {
                var UnitToDelete = _context.UnitToProducts.FirstOrDefault(a => a.ProductId == Product.ProductId);
                var UnitToAssign = _context.Units.FirstOrDefault(a => a.Name == SelectedUnit);

                _context.Remove(UnitToDelete);

                var newUnit = new UnitToProduct()
                {
                    ProductId = Product.ProductId,
                    UnitId = UnitToAssign.Id,
                    IsDeleted = false

                };
                _context.Add(newUnit);
            }
            else
            {
                var UnitToAssign = _context.Units.FirstOrDefault(a => a.Name == SelectedUnit);
                var newUnit = new UnitToProduct()
                {
                    ProductId = Product.ProductId,
                    UnitId = UnitToAssign.Id,
                    IsDeleted = false

                };
                _context.Add(newUnit);
            }

            _context.Update(Product);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostAddCat()
        {
            if (SelectedCat == null)
            {
                return NotFound();
            }

            var findCat = _context.Categories.Where(a => a.Name == SelectedCat).FirstOrDefault();
            if (findCat == null)
                return Page();

            var isAlreadyAdded = _context.CategoryToProducts.Where(a => a.ProductId == Product.ProductId && a.CategoryId == findCat.Id).FirstOrDefault();
            if (isAlreadyAdded != null)
                return Page();

            CategoryToProduct ctp = new CategoryToProduct()
            {
                CategoryId = findCat.Id,
                ProductId = Product.ProductId
            };


            _context.Add(ctp);
            await _context.SaveChangesAsync();

            return RedirectToPage("Edit", new { Id = Product.ProductId });

        }
        public async Task<IActionResult> OnPostRemoveCat(int Id)
        {
            if (Id == 0)
            {
                return Page();
            }

            var findCat = _context.CategoryToProducts.Where(a => a.CategoryId == Id && a.ProductId == Product.ProductId).FirstOrDefault();

            if (findCat == null)
                return NotFound();

            _context.Remove(findCat);
            await _context.SaveChangesAsync();

            return RedirectToPage("Edit", new { Id = Product.ProductId });

        }
    }
}
