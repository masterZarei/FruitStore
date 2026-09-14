using FS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Services.AppServices;

namespace FS.FruitStore.Pages.Payments
{
    [Authorize]
    public class ShoppingCartModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public ShoppingCartModel(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        public Factor Factor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userService.GetByUsername(User.Identity.Name).Id;

            Factor = await _orderService.GetOpenFactorAsync(userId);

            return Page();
        }
        public IActionResult OnPost(int Id)
        {
            return Redirect("ConfirmInformation");
        }
        public async Task<IActionResult> OnPostRemoveCart(int DetailId)
        {
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var result = await _orderService.DecrementDetailAsync(DetailId);
            #region Notif
            TempData["State"] = result.Success ? Notifs.Success : Notifs.Error;
            TempData["Msg"] = result.Message;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostAddToCart(int DetailId)
        {
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }

            var result = await _orderService.IncrementDetailAsync(DetailId);
            #region Notif
            TempData["State"] = result.Success ? Notifs.Success : Notifs.Error;
            TempData["Msg"] = result.Message;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostRemoveAllCart(int OrderId)
        {
            if (OrderId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var result = await _orderService.RemoveAllCartAsync(OrderId);
            if (!result)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.NOTFOUND;
                #endregion
                return RedirectToPage("/NotFound");
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
        public async Task<IActionResult> OnPostRemoveThisCart(int DetailId)
        {
            if (DetailId < 0)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            var result = await _orderService.RemoveDetailAsync(DetailId);
            if (!result.Success)
            {
                #region Notif
                TempData["State"] = Notifs.Error;
                TempData["Msg"] = Notifs.IDINVALID;
                #endregion
                return RedirectToPage("/NotFound");
            }
            #region Notif
            TempData["State"] = Notifs.Success;
            TempData["Msg"] = Notifs.SUCCEEDED;
            #endregion
            return RedirectToPage("ShoppingCart");
        }
    }
}