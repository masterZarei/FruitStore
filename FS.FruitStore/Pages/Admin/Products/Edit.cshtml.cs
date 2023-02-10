﻿using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Roles;

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
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return NotFound();
            }

            Product = await _context
                .Products
                .Where(c => c.ProductId == Id)
                .FirstOrDefaultAsync();

            if (Product == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            var checkProductUnit = new GetProductInfo(_context).GetUnit(Product.ProductId);

            if (checkProductUnit != null)
                CurrentUnit = new GetProductInfo(_context).GetUnit(Product.ProductId).Name;


            //Categories
            ProdCats = await (from a in _context.Categories
                              join b in _context.CategoryToProducts on a.Id equals b.CategoryId
                              where b.ProductId == Product.ProductId
                              select a).ToListAsync();


            Category = await (from a in _context.Categories
                              where !ProdCats.Contains(a)
                              select a).ToListAsync();

            if (Category != null)
            {
                Cats = new SelectList(Category, "Name", "Name");
            }

            // Units
            var AllUnits = _context.Units.ToList();
            if (AllUnits != null)
            {
                Units = new SelectList(AllUnits, "Name", "Name", CurrentUnit);
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.FILLREQUESTEDDATA;
                #endregion
                return Page();
            }


           

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
                var UnitToDelete = await _context
                    .UnitToProducts
                    .FirstOrDefaultAsync(a => a.ProductId == Product.ProductId);

                var UnitToAssign = await _context
                    .Units
                    .FirstOrDefaultAsync(a => a.Name == SelectedUnit);

                _context.Remove(UnitToDelete);

                var newUnit = new UnitToProduct()
                {
                    ProductId = Product.ProductId,
                    UnitId = UnitToAssign.Id
                };
                _context.Add(newUnit);
            }
            else
            {
                var UnitToAssign = await _context
                    .Units
                    .FirstOrDefaultAsync(a => a.Name == SelectedUnit);
                var newUnit = new UnitToProduct()
                {
                    ProductId = Product.ProductId,
                    UnitId = UnitToAssign.Id
                };
                _context.Add(newUnit);
            }

            _context.Update(Product);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostAddCat()
        {
            if (SelectedCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            var findCat = await _context
                .Categories
                .Where(a => a.Name == SelectedCat)
                .FirstOrDefaultAsync();

            if (findCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return Page();
            }

            var isAlreadyAdded = await _context
                .CategoryToProducts
                .Where(a => a.ProductId == Product.ProductId && a.CategoryId == findCat.Id)
                .FirstOrDefaultAsync();

            if (isAlreadyAdded != null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = "دسته بندی با همین نام برای این محصول موجود است";
                #endregion
                return Page();
            }

            CategoryToProduct ctp = new CategoryToProduct()
            {
                CategoryId = findCat.Id,
                ProductId = Product.ProductId
            };


            _context.Add(ctp);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Edit", new { Id = Product.ProductId });

        }
        public async Task<IActionResult> OnPostRemoveCat(int Id)
        {
            if (Id == 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return Page();
            }

            var findCat = await _context
                .CategoryToProducts
                .Where(a => a.CategoryId == Id && a.ProductId == Product.ProductId)
                .FirstOrDefaultAsync();

            if (findCat == null)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return NotFound();
            }

            _context.Remove(findCat);
            await _context.SaveChangesAsync();
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("Edit", new { Id = Product.ProductId });

        }
    }
}
