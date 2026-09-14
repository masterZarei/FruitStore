using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Services.AppServices;
using System.Threading.Tasks;
using Xunit;

namespace FS.Tests.Services
{
    public class WalletServiceTests
    {
        private static WalletService CreateService(FS.DataAccess.ApplicationDbContext db)
        {
            return new WalletService(db, NullLogger<WalletService>.Instance);
        }

        [Fact]
        public async Task ChargeAsync_BelowMinimum_ReturnsFailure()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر", WalletAmount = 0 });
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var result = await service.ChargeAsync("u1", 500);

            Assert.False(result.Success);
            Assert.Equal(0, (await db.Users.FindAsync("u1")).WalletAmount);
            Assert.False(await db.WalletHistories.AnyAsync());
        }

        [Fact]
        public async Task ChargeAsync_AddsBalanceAndHistory()
        {
            using var db = TestDb.Create();
            var user = new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر", WalletAmount = 1000 };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var result = await service.ChargeAsync("u1", 5000);

            Assert.True(result.Success);
            Assert.Equal(6000, user.WalletAmount);

            var history = await db.WalletHistories.SingleAsync();
            Assert.True(history.State);
            Assert.Equal(5000, history.TransactionAmount);
            Assert.Equal(6000, history.NewWalletAmount);
            Assert.InRange(history.TrackingCode, 100000000, 999999999);
            Assert.Equal(result.TrackingCode, history.TrackingCode);
        }

        [Fact]
        public async Task ChargeAsync_GeneratesUniqueTrackingCodes()
        {
            using var db = TestDb.Create();
            var user = new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var first = await service.ChargeAsync("u1", 2000);
            var second = await service.ChargeAsync("u1", 3000);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.NotEqual(first.TrackingCode, second.TrackingCode);
        }

        [Fact]
        public async Task TryDebitAsync_ReturnsFalseWhenInsufficient()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر", WalletAmount = 100 });
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var ok = await service.TryDebitAsync("u1", 200);

            Assert.False(ok);
            Assert.Equal(100, (await db.Users.FindAsync("u1")).WalletAmount);
            Assert.False(await db.WalletHistories.AnyAsync());
        }

        [Fact]
        public async Task TryDebitAsync_DeductsBalanceAndStoresDebitHistory()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر", WalletAmount = 5000 });
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var ok = await service.TryDebitAsync("u1", 1500);

            Assert.True(ok);
            Assert.Equal(3500, (await db.Users.FindAsync("u1")).WalletAmount);

            var history = await db.WalletHistories.SingleAsync();
            Assert.False(history.State);
            Assert.Equal(1500, history.TransactionAmount);
            Assert.Equal(3500, history.NewWalletAmount);
        }
    }
}