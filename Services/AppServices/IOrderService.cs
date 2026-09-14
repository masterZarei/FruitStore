using FS.Models.Models;
using System.Threading.Tasks;

namespace Services.AppServices
{
    public enum FinalizeStatus
    {
        Success,
        InvalidUser,
        EmptyCart,
        InvalidAddress,
        InsufficientWallet,
        InvalidPaymentType
    }

    public class CartChangeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class FinalizeOrderInput
    {
        public string UserId { get; set; }
        public string PaymentType { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string DeliverDate { get; set; }
        public string DeliverTime { get; set; }
    }

    public class FinalizeOrderResult
    {
        public FinalizeStatus Status { get; set; }
        public int FactorId { get; set; }
    }

    public interface IOrderService
    {
        Task<Factor> GetOpenFactorAsync(string userId);
        Task<Factor> GetOrderAsync(string userId, int factorId);
        Task<CartChangeResult> AddToCartAsync(string userId, int productId, int count, string unit);
        Task<CartChangeResult> DecrementDetailAsync(int detailId);
        Task<CartChangeResult> IncrementDetailAsync(int detailId);
        Task<CartChangeResult> RemoveDetailAsync(int detailId);
        Task<bool> RemoveAllCartAsync(int factorId);
        Task<FinalizeOrderResult> FinalizeAsync(FinalizeOrderInput input);
    }
}