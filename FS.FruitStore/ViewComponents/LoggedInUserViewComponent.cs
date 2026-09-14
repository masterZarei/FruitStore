using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.ViewComponents
{

    public class LoggedInUserViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public LoggedInUserViewComponent(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = _userService.GetByUsername(User.Identity.Name);

            LoggedInUserViewModel logged = new LoggedInUserViewModel()
            {
                Name = currentUser?.Name,
                Factor = await _context.Factors.Where(o => o.UserId == currentUser.Id && !o.IsFinally)
                .Include(o => o.FactorDetails)
                .ThenInclude(c => c.Product).FirstOrDefaultAsync()
            };

            return View("/Pages/Shared/Components/LoggedInUser.cshtml", logged);

        }
    }
}