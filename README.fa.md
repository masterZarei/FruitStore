# فروشگاه آنلاین میوه و سبزیجات

یه فروشگاه اینترنتی کامل برای خرید میوه و سبزیجات که با **ASP.NET Core Razor Pages** (net10.0) نوشته شده. امکاناتش شامل ثبت‌نام و ورود با Identity، تأیید شماره موبایل با پیامک کاوه‌نگار، سبد خرید، کیف پول، پرداخت اینترنتی، پنل کاربری و بخش مدیریت کامل محصولات و سفارشات می‌شه.

[نسخه انگلیسی / English](README.md)

## ساختار پروژه

پروژه از چند پروژه مجزا تشکیل شده که هر کدوم یه مسئولیت مشخص دارن:

```
FS/ (solution)
├── FS.FruitStore/        پروژه اصلی وب — Pagesها، Identity، DI، Serilog
├── FS.Models/            مدل‌های دیتا یا همون Entity های EF
├── DataAccess/           ApplicationDbContext و migrations
├── Services/             لایه سرویس برنامه
│   └── AppServices/      IUserService، IProductService، IWalletService، IOrderService
├── Utilities/            توابع کمکی (DateConvertor، PriceConverter، Generator و…)
└── FS.Tests/             تست‌های xUnit و تست یکپارچه با EF InMemory
```

## لایه سرویس

تو روند refactor یک لایه سرویس درست شد تا صفحات دیگه مستقیم با DbContext کار نکنن:

| سرویس | کارش |
|---|---|
| `IUserService` | کاربر رو بر اساس نام کاربری یا Id برمی‌گردونه و وضعیت فعال/غیرفعال بودنش |
| `IProductService` | واحدهای یه محصول رو برمی‌گردونه |
| `IWalletService` | افزایش موجودی کیف پول (حداقل ۱۰۰۰ تومان) و برداشت از اون. کد پیگیری ۹ رقمی یکتا هم اینجا ساخته می‌شه |
| `IOrderService` | تمام جریان خرید: افزودن به سبد، کم و زیاد کردن تعداد، حذف، و نهایی‌سازی سفارش |

### نحوه محاسبه تخفیف و مبلغ کل

- موقع نهایی‌سازی سفارش، قیمت هر آیتم با احتساب تخفیف محاسبه و ذخیره می‌شه؛ قیمت اصلی محصول داخل جدول `Product` دست نمی‌خوره.
- مبلغ کل سفارش همیشه `قیمت × تعداد`ه (نه تعداد × قیمت اصلی).
- تخفیف برای همه روش‌های پرداخت (کیف پول، در محل، اینترنتی) اعمال می‌شه.

## راه‌اندازی

### پیش‌نیازها

- .NET 10 SDK
- SQL Server (محلی یا راه دور)
- (اختیاری) ابزار dotnet-ef نسخه ۱۰

### ساخت دیتابیس

```bash
# بعد از clone، migrations رو اعمال کن:
dotnet ef database update --project DataAccess --startup-project FS.FruitStore
```

> **نکته:** جدول های migrations قبلی با یه InitialCreate تمیز جایگزین شدن. اگه دیتابیس قدیمی داری، بهترین کار اینه که دیتابیس رو drop کنی و از صفر بسازی.

### اجرا

```bash
dotnet run --project FS.FruitStore
# → http://localhost:5001
```

### تست

```bash
dotnet test FS.sln
# ۳۳ تست — تست واحد (Utilities) + تست یکپارچه سرویس‌ها روی EF InMemory
```

## خلاصه تعییرات اصلی

| مورد | توضیح |
|---|---|
| **لایه سرویس** | افزودن `IUserService`، `IProductService`، `IWalletService`، `IOrderService` و حذف استفاده مستقیم DbContext از صفحات |
| **Utilities** | کلاس‌های `GetUserInfo` و `GetProductInfo` حذف شدن و ارجاع Utilities به DataAccess قطع شد |
| **Program.cs** | Controller ها حذف شدن (فقط Razor Pages)، Serilog اضافه شد و سرویس‌ها در DI ثبت شدن |
| **Migrations** | هر ۸۷ فایل migration قدیمی با یه `InitialCreate` واحد تمیز جایگزین شد |
| **FK های قدیمی** | ستون‌های `CategoryId`، `UnitId`، `DiscountId` از جدول محصولات حذف شدن؛ رابطه چندبه‌چند حالا فقط از جداول واسط انجام می‌شه |
| **اصلاح فرایند خرید** | تخفیف و کسر موجودی بعد از موفقیت پرداخت اعمال می‌شه؛ اگه موجودی کیف پول کافی نباشه هیچ تغییری روی سفارش و محصول اعمال نمی‌شه |
| **شماره فاکتور** | به صورت عددی ساخته می‌شه (`GeneratePurchaseNumber`) — بدون Guid و بدون تغییر ساختار دیتابیس |
| **صفحه‌بندی** | صفحه همه محصولات با ۸ محصول در هر صفحه صفحه‌بندی شد |
| **تست** | پروژه xUnit اضافه شد با تست‌هایی برای Generator، تبدیل ارز و سرویس‌های کیف پول و سفارش |
| **لاگ** | Serilog با خروجی کنسول و فایل روزانه `Logs/log-*.txt` |

## ساختار صفحات

```
FS.FruitStore/Pages/
├── AllProducts.cshtml(.cs)       فهرست همه محصولات (با صفحه‌بندی)
├── Product-Details.cshtml(.cs)   صفحه محصول + دیدگاه‌ها و امتیازها
├── Payments/
│   ├── ShoppingCart.cshtml(.cs)  سبد خرید
│   ├── PaymentInfo.cshtml(.cs)   انتخاب روش پرداخت
│   └── ConfirmInformation.cshtml(.cs)  تأیید نهایی و ثبت سفارش
├── Panel/                        پنل کاربری، کیف پول، فاکتورها
└── Admin/Products/               مدیریت محصولات (ساخت، ویرایش، حذف)
```

## چند نکته

- لاگ‌های Serilog تو پوشه `Logs` ذخیره می‌شن.
- کلید کاوه‌نگار تو `appsettings.json` تحت `KavenegarSms` قرار داره — قبل از انتشار حتماً امنیتش رو بررسی کن.
- صفحات ورود و ثبت‌نام همون صفحات پیش‌فرض Identity هستن (`/Identity/Account/Login` و `/Identity/Account/Register`).