using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.AppServices;

namespace FS.FruitStore.Pages.Panel
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult OnGet()
        {
            #region isDisabled?
            if (_userService.IsDisabled(User.Identity.Name))
                return Redirect("/Identity/Account/AccessDenied");
            #endregion
            return Page();
        }
    }
}