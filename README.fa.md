# فروشگاه آنلاین میوه و سبزیجات

فروشگاه آنلاین میوه و سبزیجات با ASP.NET Core Razor Pages (نسخه net10.0) به همراه SQL Server، ASP.NET Identity، تأیید پیامکی کاوه‌نگار (Kavenegar) و ثبت گزارش با Serilog.

[نسخه انگلیسی / English](README.md)

## معماری برنامه

```
FS/ (حل solution)
├── FS.FruitStore/        میزبان وب ASP.NET Core — Razor Pages، Identity، DI، Serilog
├── FS.Models/            کلاس‌های موجودیت (POCO) + ویژگی‌های EF (Product، User، Factor، FactorDetail، WalletHistory و…)
├── DataAccess/           DbContext و مایگریشن‌های EF Core (SQL Server)
├── Services/             سرویس‌های لایه کاربردی (Application Layer) ثبت‌شده از طریق DI
│   └── AppServices/      IUserService، IProductService، IWalletService، IOrderService
├── Utilities/            کلاس‌های کمکی (DateConvertor، PriceConverter، Generator و…)
└── FS.Tests/             تست‌های واحد xUnit + تست یکپارچه InMemory
```

### لایه سرویس (ایجادشده در بازآرایی/refactor)

| سرویس | وظایف |
|---|---|
| `IUserService` | `GetByUsername`، `GetById`، `IsDisabled` |
| `IProductService` | `GetUnitsByProductAsync` |
| `IWalletService` | `ChargeAsync` (حداقل ۱۰۰۰ تومان)، `TryDebitAsync` (اتمیک همراه با کد پیگیری ۹ رقمی یکتا و مقاوم در برابر تصادم) |
| `IOrderService` | جریان کامل خرید: `AddToCartAsync`، `DecrementDetailAsync`، `IncrementDetailAsync`، `RemoveDetailAsync`، `RemoveAllCartAsync`، `GetOpenFactorAsync`، `GetOrderAsync`، **`FinalizeAsync`** (تخفیف با `Price×Count`، برداشت از کیف پول، تولید شماره فاکتور، اطلاعات ارسال کاربر، کاهش موجودی) |

### محاسبه تخفیف و مبلغ کل

- `FactorDetail.Price` **فقط هنگام نهایی‌سازی سفارش** با قیمت واحد تخفیف‌خورده بازنویسی می‌شود (`DiscountApplier.Apply`)؛ `Product.Price` هرگز تغییر نمی‌کند.
- `Factor.Total = Σ (قیمت تخفیف‌خورده × تعداد)` — همیشه `قیمت×تعداد`، نه `تعداد×قیمت اصلی`.
- تخفیف برای **همه روش‌های پرداخت** (کیف پول، پرداخت در محل، پرداخت اینترنتی) اعمال می‌شود.

## راه‌اندازی

### پیش‌نیازها

- SDK دات‌نت ۱۰ (۱۰٫۰٫۴۰۱ به بالا)
- SQL Server (محلی یا راه دور)
- (اختیاری) ابزار `dotnet ef` نسخه ۱۰: `dotnet tool install --global dotnet-ef`

### پایگاه داده

```bash
# پس از کلون، مایگریشن‌ها را اعمال کنید (یا دیتابیس را از صفر بسازید):
dotnet ef database update --project DataAccess --startup-project FS.FruitStore
```

> **مهم:** مایگریشن اولیه نشان‌دهنده **کل** اسکیمای تمیز است (بدون ستون‌های FK قدیمی جدول Products). اگر از دیتابیس قدیمی مهاجرت می‌کنید، به‌جای اعمال InitialCreate روی تاریخچه قدیمی، **دیتابیس را حذف و دوباره بسازید**.

### اجرا

```bash
dotnet run --project FS.FruitStore
# → http://localhost:5001
```

### اجرای تست‌ها

```bash
dotnet test FS.sln
# 33 تست — تست واحد (Utilities) + تست بدون یکپارچه‌سازی (OrderService، WalletService، UserService روی EF InMemory)
```

## بهبودهای کلیدی (خلاصه بازآرایی)

| مورد | تغییر |
|---|---|
| **لایه سرویس** | `IUserService`، `IProductService`، `IWalletService`، `IOrderService` جایگزین استفاده مستقیم `GetUserInfo` / `GetProductInfo` و `_db` در صفحات شدند |
| **Utilities** | کلاس‌های `GetUserInfo` / `GetProductInfo` حذف شدند؛ `Utilities.csproj` دیگر به `DataAccess` ارجاع نمی‌دهد |
| **Program.cs** | کنترلرها حذف شدند (فقط Razor Pages)؛ Serilog (کنسول + فایل `Logs/log-.txt`) ثبت شد؛ چهار سرویس ثبت شدند |
| **تک‌تکه‌کردن مایگریشن‌ها** | هر ۸۷ فایل قدیمی با یک `InitialCreate` تمیز جایگزین شدند |
| **FKهای قدیمی Product** | ستون‌های shadow `CategoryId`، `UnitId`، `DiscountId` از جدول Products حذف شدند (رابطه چندبه‌چند حالا فقط از جداول میانی `CategoryToProducts` و `UnitToProducts` استفاده می‌کند) |
| **اصلاح فرایند خرید** | `OrderService.FinalizeAsync`: محاسبه تخفیف *قبل از* برداشت کیف پول (بدون تغییر در حافظه در صورت شکست)، مبلغ کل = Σ قیمت تخفیف‌خورده×تعداد، کاهش موجودی *بعد از* موفقیت پرداخت |
| **کیف پول** | `IWalletService.ChargeAsync` (حداقل ۱۰۰۰ تومان)، `TryDebitAsync`؛ کد پیگیری ۹ رقمی یکتا با `Generator.GenerateSecureDigits(9)` و تلاش مجدد هنگام تصادم در دیتابیس |
| **شماره فاکتور** | رشته عددی (`Generator.GeneratePurchaseNumber()`) — بدون Guid و بدون تغییر ساختار دیتابیس |
| **تخفیف** | برای همه روش‌های پرداخت اعمال می‌شود؛ قیمت واحد تخفیف‌خورده در `FactorDetail.Price` ذخیره می‌شود؛ قیمت محصول تغییر نمی‌کند |
| **صفحه‌بندی** | صفحه `AllProducts` با ۸ محصول در هر صفحه و با استفاده از `FS.Models.Paging.PagingInfo` صفحه‌بندی شد؛ `PagingInfo` نگه داشته شد (حذف نشد) |
| **تست‌ها** | پروژه xUnit `FS.Tests`: `DiscountApplierTests`، `PriceConverterTests`، `DateConvertorTests`، `GeneratorTests`، `UserServiceTests`، `WalletServiceTests`، `OrderServiceTests` (همه در حال عبور) |
| **ثبت گزارش (Logging)** | Serilog کنسول + فایل روزانه (`Logs/log-*.txt`) |

## ساختار پروژه (بعد از بازآرایی)

```
FS.FruitStore/
├── Program.cs                        میزبان + DI + Serilog
├── Pages/
│   ├── AllProducts.cshtml(.cs)       فهرست محصولات با صفحه‌بندی
│   ├── Product-Details.cshtml(.cs)   نمایش محصول + دیدگاه‌ها و امتیازها
│   ├── Payments/
│   │   ├── ShoppingCart.cshtml(.cs)
│   │   ├── PaymentInfo.cshtml(.cs)
│   │   └── ConfirmInformation.cshtml(.cs)   پرداخت → OrderService.FinalizeAsync
│   ├── Panel/                         داشبورد کاربر، کیف پول، فاکتورها
│   └── Admin/Products/                عملیات CRUD (از نظر سرویس بدون تغییر)
├── ViewComponents/
│   └── LoggedInUserViewComponent.cs   استفاده از IUserService

Services/
├── Services.csproj                    ارجاع به DataAccess، FS.Models، Utilities
├── AppServices/
│   ├── IUserService.cs + UserService.cs
│   ├── IProductService.cs + ProductService.cs
│   ├── IWalletService.cs + WalletService.cs
│   └── IOrderService.cs + OrderService.cs

DataAccess/
├── ApplicationDbContext.cs
└── Migrations/
    └── *_InitialCreate.cs            اسکیمای کامل و تمیز

FS.Tests/
├── TestDb.cs                         ساخت InMemory
├── Utilities/                        DiscountApplierTests، PriceConverterTests، DateConvertorTests، GeneratorTests
└── Services/                         UserServiceTests، WalletServiceTests، OrderServiceTests
```

## نکات

- **مسیر ورودهای Serilog:** `Logs/log-<date>.txt` (نسبی به پوشه اجرا).
- **پیامک:** API کاوه‌نگار — اطلاعات اعتبارسنجی در `appsettings.json` (`KavenegarSms`) قرار دارد. در صورت انتشار، حذف یا تغییر دهید.
- **Identity:** صفحات Identity مبتنی بر Razor Pages (`/Identity/Account/Login`، `/Identity/Account/Register`).