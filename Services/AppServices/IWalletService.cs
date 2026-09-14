using System.Threading.Tasks;

namespace Services.AppServices
{
    public class WalletChargeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TrackingCode { get; set; }
    }

    public interface IWalletService
    {
        Task<WalletChargeResult> ChargeAsync(string userId, double amount);
        Task<bool> TryDebitAsync(string userId, double amount);
    }
}