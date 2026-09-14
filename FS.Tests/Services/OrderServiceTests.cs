using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Services.AppServices;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FS.Tests.Services
{
    public class OrderServiceTests
    {
        private static (OrderService orders, ApplicationDbContext db) Setup()
        {
            var db = TestDb.Create();
            var wallets = new WalletService(db, NullLogger<WalletService>.Instance);
            var orders = new OrderService(db, wallets, NullLogger<OrderService>.Instance);
            return (orders, db);
        }

        private static async Task AddScenarioAsync(ApplicationDbContext db, double walletAmount = 0)
        {
            db.Users.Add(new User
            {
                Id = "u1",
                UserName = "u1", Name = "تست", LastName = "کاربر",
                WalletAmount = walletAmount,
                Address = "تهران",
                PostalCode = "11111"
            });
            db.Products.Add(new Product
            {
                ProductId = 1,
                Name = "سیب",
                Description = "سیب قرمز",
                Price = 2000,
                Count = 10,
                Discount = 10,
                isVerified = true
            });
            db.Factors.Add(new Factor { FactorId = 1, UserId = "u1", IsFinally = false });
            db.FactorDetails.Add(new FactorDetail
            {
                DetailId = 1,
                FactorId = 1,
                ProductId = 1,
                Price = 2000,
                Count = 3,
                Unit = "kg"
            });
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task Finalize_CashOnDelivery_AppliesDiscountKeepsProductPriceAndDecrementsStock()
        {
            var (orders, db) = Setup();
            await AddScenarioAsync(db, walletAmount: 0);

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u1",
                PaymentType = "پرداخت در محل",
                PostalCode = "12345",
                Address = "خیابان آزادی",
                DeliverDate = "1403/01/01",
                DeliverTime = "صبح تا ظهر 9 - 12"
            });

            Assert.Equal(FinalizeStatus.Success, result.Status);
            Assert.Equal(1, result.FactorId);

            var factor = await db.Factors.FirstAsync();
            var detail = await db.FactorDetails.FirstAsync();
            var product = await db.Products.FirstAsync();

            Assert.True(factor.IsFinally);
            Assert.Equal("پرداخت در محل", factor.Payment_Type);
            Assert.Equal((byte)1, factor.DeliverState);
            Assert.False(string.IsNullOrEmpty(factor.PurchaseNumber));
            Assert.True(factor.PurchaseNumber.All(char.IsDigit));

            // discount applied to line item: 2000 - 10% = 1800
            Assert.Equal(1800, detail.Price);
            // original product price never mutated
            Assert.Equal(2000, product.Price);
            // stock decremented: 10 - 3
            Assert.Equal(7, product.Count);
            // user delivery info updated (PostalCode duplicate-assignment bug fixed)
            var user = await db.Users.FindAsync("u1");
            Assert.Equal("12345", user.PostalCode);
            Assert.Equal("خیابان آزادی", user.Address);
        }

        [Fact]
        public async Task Finalize_WalletPayment_DebitsCorrectTotalPriceTimesCount()
        {
            var (orders, db) = Setup();
            await AddScenarioAsync(db, walletAmount: 6000);

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u1",
                PaymentType = "پرداخت با کیف پول",
                PostalCode = "22222",
                Address = "شیراز"
            });

            Assert.Equal(FinalizeStatus.Success, result.Status);

            // total = discountedPrice(1800) * count(3) = 5400 -> balance 6000 - 5400 = 600
            var user = await db.Users.FindAsync("u1");
            Assert.Equal(600, user.WalletAmount);

            var history = await db.WalletHistories.SingleAsync();
            Assert.False(history.State);
            Assert.Equal(5400, history.TransactionAmount);
            Assert.Equal(600, history.NewWalletAmount);
            Assert.InRange(history.TrackingCode, 100000000, 999999999);

            var product = await db.Products.FirstAsync();
            Assert.Equal(2000, product.Price);
            Assert.Equal(7, product.Count);
        }

        [Fact]
        public async Task Finalize_WalletPayment_InsufficientBalance_DoesNotFinalize()
        {
            var (orders, db) = Setup();
            await AddScenarioAsync(db, walletAmount: 100);

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u1",
                PaymentType = "پرداخت با کیف پول",
                PostalCode = "22222",
                Address = "شیراز"
            });

            Assert.Equal(FinalizeStatus.InsufficientWallet, result.Status);

            var factor = await db.Factors.FirstAsync();
            var product = await db.Products.FirstAsync();
            var user = await db.Users.FindAsync("u1");

            Assert.False(factor.IsFinally);
            Assert.Equal(10, product.Count); // stock untouched
            Assert.Equal(2000, product.Price); // price untouched
            Assert.Equal(100, user.WalletAmount);
            Assert.False(await db.WalletHistories.AnyAsync());
        }

        [Fact]
        public async Task Finalize_InvalidAddress_ReturnsInvalidAddress()
        {
            var (orders, db) = Setup();
            await AddScenarioAsync(db);

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u1",
                PaymentType = "پرداخت در محل",
                PostalCode = "",
                Address = ""
            });

            Assert.Equal(FinalizeStatus.InvalidAddress, result.Status);
            Assert.False((await db.Factors.FirstAsync()).IsFinally);
        }

        [Fact]
        public async Task Finalize_EmptyCart_ReturnsEmptyCart()
        {
            var (orders, db) = Setup();
            db.Users.Add(new User { Id = "u2", UserName = "u2", Name = "تست", LastName = "کاربر", Address = "x", PostalCode = "1" });
            await db.SaveChangesAsync();

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u2",
                PaymentType = "پرداخت در محل",
                PostalCode = "1",
                Address = "x"
            });

            Assert.Equal(FinalizeStatus.EmptyCart, result.Status);
        }

        [Fact]
        public async Task Finalize_InvalidPaymentType_ReturnsInvalid()
        {
            var (orders, db) = Setup();
            await AddScenarioAsync(db);

            var result = await orders.FinalizeAsync(new FinalizeOrderInput
            {
                UserId = "u1",
                PaymentType = "نامشخص",
                PostalCode = "1",
                Address = "x"
            });

            Assert.Equal(FinalizeStatus.InvalidPaymentType, result.Status);
            Assert.False((await db.Factors.FirstAsync()).IsFinally);
        }

        [Fact]
        public async Task AddToCart_CreatesNewFactorAndDetailWhenCartIsEmpty()
        {
            var (orders, db) = Setup();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر" });
            db.Products.Add(new Product { ProductId = 1, Name = "هندوانه", Description = "توضیح", Price = 500, Count = 5 });
            await db.SaveChangesAsync();

            var result = await orders.AddToCartAsync("u1", 1, 2, "عدد");

            Assert.True(result.Success);
            var factor = await db.Factors.FirstAsync();
            Assert.False(factor.IsFinally);
            var detail = await db.FactorDetails.FirstAsync();
            Assert.Equal(2, detail.Count);
            Assert.Equal(500, detail.Price);
            Assert.Equal("عدد", detail.Unit);
        }

        [Fact]
        public async Task AddToCart_IncrementsCountOfExistingDetail()
        {
            var (orders, db) = Setup();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر" });
            db.Products.Add(new Product { ProductId = 1, Name = "موز", Description = "توضیح", Price = 1000, Count = 5 });
            db.Factors.Add(new Factor { FactorId = 1, UserId = "u1", IsFinally = false });
            db.FactorDetails.Add(new FactorDetail { DetailId = 1, FactorId = 1, ProductId = 1, Price = 1000, Count = 1 });
            await db.SaveChangesAsync();

            var result = await orders.AddToCartAsync("u1", 1, 3, "kg");

            Assert.True(result.Success);
            Assert.Equal(4, (await db.FactorDetails.FirstAsync()).Count);
        }

        [Fact]
        public async Task AddToCart_RejectsCountAboveStock()
        {
            var (orders, db) = Setup();
            db.Users.Add(new User { Id = "u1", UserName = "u1", Name = "تست", LastName = "کاربر" });
            db.Products.Add(new Product { ProductId = 1, Name = "گیلاس", Description = "توضیح", Price = 1000, Count = 2 });
            await db.SaveChangesAsync();

            var result = await orders.AddToCartAsync("u1", 1, 5, "kg");

            Assert.False(result.Success);
            Assert.False(await db.Factors.AnyAsync());
        }
    }
}