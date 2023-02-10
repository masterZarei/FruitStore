using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Roles;

namespace FS.FruitStore.Pages.Admin.CommentManagement
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<CommentsIndexVM> Comments { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            var CommentsList = await _db.Comments
                .Where(a => a.Answer == null)
                .Include(a => a.Product)
                .ToListAsync();
            Comments = new List<CommentsIndexVM>();

           

            foreach (var item in CommentsList)
            {
                Comments.Add(new CommentsIndexVM {
                    productId = item.Product_Id,
                    Name = item.Product.Name,
                    CommentsCount = CommentsList.Count 
                });
            }

            for (int i = 0; i < Comments.Count; i++)
            {
                for (int j = 0; j < Comments.Count; j++)
                {
                    if (i != j && (Comments[j].productId == Comments[i].productId))
                    {
                        Comments.Remove(Comments[i]);
                    }
                }
            }


            return Page();
        }
    }
}
