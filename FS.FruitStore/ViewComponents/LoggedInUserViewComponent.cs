using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Convertors;

namespace Mahshop.ViewComponents
{

    public class LoggedInUserViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LoggedInUserViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            GetUserInfo mtd = new GetUserInfo(_context);

            LoggedInUserViewModel logged = new LoggedInUserViewModel()
            {
                Name = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Name,
                Factor = await _context.Factors.Where(o => o.UserId == (mtd.GetInfoByUsername(User.Identity.Name).Id) && !o.IsFinally)
                .Include(o => o.FactorDetails)
                .ThenInclude(c => c.Product).FirstOrDefaultAsync()
            };

            return View("/Pages/Shared/Components/LoggedInUser.cshtml", logged);

        }
    }
}
