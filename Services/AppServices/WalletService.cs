using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Utilities;

namespace Services.AppServices
{
    public class WalletService : IWalletService
    {
        private const double MinimumChargeAmount = 1000;

        private readonly ApplicationDbContext _db;
        private readonly ILogger<WalletService> _logger;

        public WalletService(ApplicationDbContext db, ILogger<WalletService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<WalletChargeResult> ChargeAsync(string userId, double amount)
        {
            if (amount < MinimumChargeAmount)
            {
                return new WalletChargeResult
                {
                    Success = false,
                    Message = "لطفا مبلغی بالاتر از هزارتومان وارد کنید"
                };
            }

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Wallet charge failed: user {UserId} not found", userId);
                return new WalletChargeResult { Success = false, Message = "کاربر یافت نشد" };
            }

            user.WalletAmount += amount;

            var history = new WalletHistory
            {
                NewWalletAmount = user.WalletAmount,
                State = true,
                TrackingCode = await GenerateUniqueTrackingCodeAsync(),
                UserId = user.Id,
                TransactionAmount = amount
            };

            _db.WalletHistories.Add(history);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Wallet charged for user {UserId}: +{Amount} -> new balance {Balance}, tracking {TrackingCode}",
                user.Id, amount, user.WalletAmount, history.TrackingCode);

            return new WalletChargeResult
            {
                Success = true,
                TrackingCode = history.TrackingCode
            };
        }

        public async Task<bool> TryDebitAsync(string userId, double amount)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.WalletAmount < amount)
            {
                _logger.LogWarning("Wallet debit failed for user {UserId}: balance {Balance} < requested {Amount}",
                    userId, user?.WalletAmount, amount);
                return false;
            }

            user.WalletAmount -= amount;

            var history = new WalletHistory
            {
                NewWalletAmount = user.WalletAmount,
                State = false,
                TrackingCode = await GenerateUniqueTrackingCodeAsync(),
                UserId = user.Id,
                TransactionAmount = amount
            };

            _db.WalletHistories.Add(history);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Wallet debited for user {UserId}: -{Amount} -> new balance {Balance}, tracking {TrackingCode}",
                user.Id, amount, user.WalletAmount, history.TrackingCode);

            return true;
        }

        private async Task<int> GenerateUniqueTrackingCodeAsync()
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int code = Generator.GenerateSecureDigits(9);
                bool exists = await _db.WalletHistories.AnyAsync(w => w.TrackingCode == code);
                if (!exists)
                    return code;
            }

            // Extremely unlikely fallback: count-based code guarantees an unused number.
            return (int)(100000000 + await _db.WalletHistories.CountAsync());
        }
    }
}