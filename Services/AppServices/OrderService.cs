using FS.DataAccess;
using FS.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Convertors;

namespace Services.AppServices
{
    public class OrderService : IOrderService
    {
        private const string PayOnDelivery = "پرداخت در محل";
        private const string PayOnline = "پرداخت اینترنتی";
        private const string PayWithWallet = "پرداخت با کیف پول";

        private readonly ApplicationDbContext _db;
        private readonly IWalletService _walletService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext db, IWalletService walletService, ILogger<OrderService> logger)
        {
            _db = db;
            _walletService = walletService;
            _logger = logger;
        }

        public async Task<Factor> GetOpenFactorAsync(string userId)
        {
            return await _db.Factors
                .Where(o => o.UserId == userId && o.IsFinally == false)
                .Include(o => o.FactorDetails)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<Factor> GetOrderAsync(string userId, int factorId)
        {
            return await _db.Factors
                .Where(a => a.FactorId == factorId && a.UserId == userId)
                .Include(a => a.FactorDetails)
                .ThenInclude(a => a.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<CartChangeResult> AddToCartAsync(string userId, int productId, int count, string unit)
        {
            var currentProduct = await _db.Products
                .FirstOrDefaultAsync(a => a.ProductId == productId);

            if (currentProduct == null)
                return new CartChangeResult { Success = false, Message = Notifs.NOTFOUND };

            var factor = await _db.Factors
                .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsFinally);

            if (factor != null)
            {
                var factorDetail = await _db.FactorDetails
                    .FirstOrDefaultAsync(f => f.FactorId == factor.FactorId &&
                                              f.ProductId == currentProduct.ProductId);

                if (factorDetail != null)
                {
                    if (currentProduct.Count >= factorDetail.Count + count)
                    {
                        factorDetail.Count += count;
                    }
                    else
                    {
                        return new CartChangeResult
                        {
                            Success = false,
                            Message = "لطفا به تعداد موجود ،محصول به سبد خریدتان اضافه نمایید."
                        };
                    }
                }
                else
                {
                    if (currentProduct.Count >= count)
                    {
                        _db.FactorDetails.Add(new FactorDetail
                        {
                            FactorId = factor.FactorId,
                            ProductId = currentProduct.ProductId,
                            Price = currentProduct.Price,
                            Count = count,
                            Unit = unit
                        });
                    }
                    else
                    {
                        return new CartChangeResult
                        {
                            Success = false,
                            Message = "لطفا به تعداد موجود ،محصول به سبد خریدتان اضافه نمایید."
                        };
                    }
                }
            }
            else
            {
                if (currentProduct.Count >= count)
                {
                    factor = new Factor { IsFinally = false, UserId = userId };
                    _db.Factors.Add(factor);
                    await _db.SaveChangesAsync();

                    _db.FactorDetails.Add(new FactorDetail
                    {
                        FactorId = factor.FactorId,
                        ProductId = currentProduct.ProductId,
                        Price = currentProduct.Price,
                        Count = count,
                        Unit = unit
                    });
                }
                else
                {
                    return new CartChangeResult
                    {
                        Success = false,
                        Message = "لطفا به تعداد موجود،محصول به سبد خریدتان اضافه نمایید."
                    };
                }
            }

            await _db.SaveChangesAsync();
            return new CartChangeResult { Success = true, Message = Notifs.SUCCEEDED };
        }

        public async Task<CartChangeResult> DecrementDetailAsync(int detailId)
        {
            var factorDetail = await _db.FactorDetails.FindAsync(detailId);
            if (factorDetail == null)
                return new CartChangeResult { Success = false, Message = Notifs.IDINVALID };

            if (factorDetail.Count > 1)
            {
                factorDetail.Count -= 1;
                await _db.SaveChangesAsync();
                return new CartChangeResult { Success = true, Message = Notifs.SUCCEEDED };
            }

            _db.FactorDetails.Remove(factorDetail);

            var checkFactor = await _db.Factors
                .Include(a => a.FactorDetails)
                .ThenInclude(a => a.Product)
                .FirstOrDefaultAsync(a => a.FactorId == factorDetail.FactorId);

            if (checkFactor != null)
            {
                int check = checkFactor.FactorDetails
                    .Where(a => a.Product.Count > 0)
                    .Count();

                if (check < 1)
                    _db.Factors.Remove(checkFactor);
            }

            await _db.SaveChangesAsync();
            return new CartChangeResult { Success = true, Message = Notifs.SUCCEEDED };
        }

        public async Task<CartChangeResult> IncrementDetailAsync(int detailId)
        {
            var factorDetail = await _db.FactorDetails
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.DetailId == detailId);

            if (factorDetail == null)
                return new CartChangeResult { Success = false, Message = Notifs.IDINVALID };

            if (factorDetail.Count <= factorDetail.Product.Count - 1)
            {
                factorDetail.Count += 1;
                await _db.SaveChangesAsync();
                return new CartChangeResult { Success = true, Message = Notifs.SUCCEEDED };
            }

            return new CartChangeResult
            {
                Success = false,
                Message = "موجودیت محصول کمتر از مقدار خواسته شده می باشد."
            };
        }

        public async Task<CartChangeResult> RemoveDetailAsync(int detailId)
        {
            var orderDetail = await _db.FactorDetails.FindAsync(detailId);
            if (orderDetail == null)
                return new CartChangeResult { Success = false, Message = Notifs.IDINVALID };

            var factor = await _db.Factors
                .Include(a => a.FactorDetails)
                .FirstOrDefaultAsync(a => a.FactorId == orderDetail.FactorId);

            if (factor == null)
                return new CartChangeResult { Success = false, Message = Notifs.IDINVALID };

            if (factor.FactorDetails.Count <= 1)
            {
                _db.Factors.Remove(factor);
            }
            else
            {
                _db.FactorDetails.Remove(orderDetail);
            }

            await _db.SaveChangesAsync();
            return new CartChangeResult { Success = true, Message = Notifs.SUCCEEDED };
        }

        public async Task<bool> RemoveAllCartAsync(int factorId)
        {
            var factor = await _db.Factors.FindAsync(factorId);
            if (factor == null)
                return false;

            _db.Factors.Remove(factor);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<FinalizeOrderResult> FinalizeAsync(FinalizeOrderInput input)
        {
            var factor = await _db.Factors
                .Include(a => a.FactorDetails)
                .ThenInclude(b => b.Product)
                .Where(a => a.UserId == input.UserId && !a.IsFinally)
                .FirstOrDefaultAsync();

            if (factor == null || factor.FactorDetails.Count < 1)
                return new FinalizeOrderResult { Status = FinalizeStatus.EmptyCart };

            var user = await _db.Users.FirstOrDefaultAsync(a => a.Id == input.UserId);
            if (user == null)
                return new FinalizeOrderResult { Status = FinalizeStatus.InvalidUser };

            if (string.IsNullOrEmpty(input.PostalCode) || string.IsNullOrEmpty(input.Address))
                return new FinalizeOrderResult { Status = FinalizeStatus.InvalidAddress, FactorId = factor.FactorId };

            // Compute the correct total (Price * Count) WITHOUT mutating anything, so a failed
            // wallet debit leaves the factor, stock and prices untouched.
            double total = 0;
            foreach (var item in factor.FactorDetails)
            {
                var product = item.Product;
                if (product != null && product.Discount > 0)
                    total += DiscountApplier.Apply(item.Price, product.Discount) * item.Count;
                else
                    total += item.Price * item.Count;
            }

            switch (input.PaymentType)
            {
                case PayOnDelivery:
                case PayOnline:
                    break;

                case PayWithWallet:
                    bool debited = await _walletService.TryDebitAsync(input.UserId, total);
                    if (!debited)
                        return new FinalizeOrderResult { Status = FinalizeStatus.InsufficientWallet };
                    break;

                default:
                    return new FinalizeOrderResult { Status = FinalizeStatus.InvalidPaymentType };
            }

            // Apply per-product discount to every line item and decrement stock.
            // The original product price is never mutated.
            foreach (var item in factor.FactorDetails)
            {
                var product = item.Product;
                if (product != null && product.Discount > 0)
                    item.Price = DiscountApplier.Apply(item.Price, product.Discount);

                if (product != null)
                    product.Count -= item.Count;
            }

            user.PostalCode = input.PostalCode;
            user.Address = input.Address;

            factor.Payment_Type = input.PaymentType;
            factor.Description = input.Description;
            factor.DeliverState = 1;
            factor.Deliver_Date = input.DeliverDate;
            factor.Deliver_Time = input.DeliverTime;
            factor.PurchaseNumber = Generator.GeneratePurchaseNumber();
            factor.IsFinally = true;

            _db.Factors.Update(factor);
            _db.Users.Update(user);

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Order {FactorId} finalized for user {UserId}: payment {PaymentType}, total {Total}, purchase number {PurchaseNumber}",
                factor.FactorId, input.UserId, input.PaymentType, total, factor.PurchaseNumber);

            return new FinalizeOrderResult { Status = FinalizeStatus.Success, FactorId = factor.FactorId };
        }
    }
}