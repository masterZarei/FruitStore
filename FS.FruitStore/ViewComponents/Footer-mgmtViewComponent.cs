using FS.DataAccess;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Mahshop.ViewComponents
{

    public class FooterMgmtViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FooterMgmtViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            FooterMgmtViewModel logged = new FooterMgmtViewModel()
            {
                Footer = await _context.Footers.FirstOrDefaultAsync(),
                ContactWays = await _context.ContactWays.Where(a=>a.IsInFooter.Equals(true)).ToListAsync()
            };

            return View("/Pages/Shared/Components/FooterMgmt.cshtml", logged);

        }
    }
}
