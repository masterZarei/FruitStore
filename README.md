# Online FruitStore

ASP.NET Core Razor Pages (net10.0) online fruit/vegetable store with SQL Server, ASP.NET Identity, Kavenegar SMS verification, and Serilog logging.

## Architecture

```
FS/ (solution)
├── FS.FruitStore/        ASP.NET Core web host — Razor Pages, Identity, DI, Serilog
├── FS.Models/            POCOs + EF annotations (Product, User, Factor, FactorDetail, WalletHistory …)
├── DataAccess/            EF Core DbContext + migrations (SQL Server)
├── Services/              Application-layer services registered via DI
│   └── AppServices/       IUserService, IProductService, IWalletService, IOrderService
├── Utilities/             Helpers (DateConvertor, PriceConverter, Generator …)
└── FS.Tests/              xUnit unit + InMemory integration tests
```

### Service Layer (created during refactor)

| Service | Responsibilities |
|---|---|
| `IUserService` | `GetByUsername`, `GetById`, `IsDisabled` |
| `IProductService` | `GetUnitsByProductAsync` |
| `IWalletService` | `ChargeAsync` (min 1000 Toman), `TryDebitAsync` (atomic with collision-safe unique 9-digit tracking code) |
| `IOrderService` | Full checkout flow: `AddToCartAsync`, `DecrementDetailAsync`, `IncrementDetailAsync`, `RemoveDetailAsync`, `RemoveAllCartAsync`, `GetOpenFactorAsync`, `GetOrderAsync`, **`FinalizeAsync`** (discount via `Price*Count`, wallet debit, `PurchaseNumber` generation, user delivery info, stock decrement) |

### Discount & Total Calculation

- `FactorDetail.Price` is overwritten with the discounted unit price (`DiscountApplier.Apply`) **only at finalize time**; `Product.Price` is never mutated.
- `Factor.Total = Σ (DiscountedPrice × Quantity)` — always Price×Count, never Quantity×OriginalPrice.
- Discount is applied for **all payment types** (wallet, COD, online).

## Setup

### Prerequisites

- .NET 10 SDK (10.0.401+)
- SQL Server (local or remote)
- (optional) `dotnet ef` tool v10: `dotnet tool install --global dotnet-ef`

### Database

```bash
# after cloning, apply migrations (or recreate from scratch):
dotnet ef database update --project DataAccess --startup-project FS.FruitStore
```

> **Important:** The initial migration represents the **full** clean schema (no legacy Product FK columns). If migrating from the old DB, **drop and recreate** the database rather than trying to apply the new InitialCreate over the old history.

### Run

```bash
dotnet run --project FS.FruitStore
# → http://localhost:5001
```

### Run Tests

```bash
dotnet test FS.sln
# 33 tests — unit (Utilities) + integration (OrderService, WalletService, UserService on EF InMemory)
```

## Key Improvements (refactor summary)

| Item | Change |
|---|---|
| **Service layer** | `IUserService`, `IProductService`, `IWalletService`, `IOrderService` replace all `GetUserInfo` / `GetProductInfo` new-injection and `_db` usage in pages |
| **Utilities** | `GetUserInfo` / `GetProductInfo` removed; `Utilities.csproj` no longer references `DataAccess` |
| **Program.cs** | Controllers removed (Razor Pages only); Serilog (console + file `Logs/log-.txt`) registered; four services registered |
| **Migrations squashed** | All 87 old files replaced by a single clean `InitialCreate` |
| **Product legacy FKs** | `CategoryId`, `UnitId`, `DiscountId` shadow FK columns removed from Products (many-to-many now uses `CategoryToProducts` / `UnitToProducts` pivots only) |
| **Checkout fix** | `OrderService.FinalizeAsync`: discount computed *before* wallet debit (no in-memory mutation on failure), total = Σ discountedPrice×count, stock decremented *after* payment succeeds |
| **Wallet** | `IWalletService.ChargeAsync` (min 1000 Toman), `TryDebitAsync`; unique 9-digit tracking code via `Generator.GenerateSecureDigits(9)` with DB-collision retry |
| **PurchaseNumber** | Numeric string (`Generator.GeneratePurchaseNumber()`) — no Guid, no schema change |
| **Discount** | Applied to every payment type; discounted unit price stored on `FactorDetail.Price`; product price unchanged |
| **Paging** | `AllProducts` paginated (8 items/page) via `FS.Models.Paging.PagingInfo`; `PagingInfo` kept (not deleted) |
| **Tests** | `FS.Tests` xUnit project: `DiscountApplierTests`, `PriceConverterTests`, `DateConvertorTests`, `GeneratorTests`, `UserServiceTests`, `WalletServiceTests`, `OrderServiceTests` (all passing) |
| **Logging** | Serilog console + rolling daily file (`Logs/log-*.txt`) |

## Project Structure (post-refactor)

```
FS.FruitStore/
├── Program.cs                        Host + DI + Serilog
├── Pages/
│   ├── AllProducts.cshtml(.cs)       Product listing with pagination
│   ├── Product-Details.cshtml(.cs)   Single product + comments/ratings
│   ├── Payments/
│   │   ├── ShoppingCart.cshtml(.cs)
│   │   ├── PaymentInfo.cshtml(.cs)
│   │   └── ConfirmInformation.cshtml(.cs)   Checkout → OrderService.FinalizeAsync
│   ├── Panel/                         User dashboard, wallet, factors
│   └── Admin/Products/                CRUD (unchanged service-wise)
├── ViewComponents/
│   └── LoggedInUserViewComponent.cs   Uses IUserService

Services/
├── Services.csproj                    References DataAccess, FS.Models, Utilities
├── AppServices/
│   ├── IUserService.cs + UserService.cs
│   ├── IProductService.cs + ProductService.cs
│   ├── IWalletService.cs + WalletService.cs
│   └── IOrderService.cs + OrderService.cs

DataAccess/
├── ApplicationDbContext.cs
└── Migrations/
    └── *_InitialCreate.cs            Full clean schema

FS.Tests/
├── TestDb.cs                         InMemory factory
├── Utilities/                        DiscountApplierTests, PriceConverterTests, DateConvertorTests, GeneratorTests
└── Services/                         UserServiceTests, WalletServiceTests, OrderServiceTests
```

## Notes

- **Serilog** log path: `Logs/log-<date>.txt` (relative to working directory).
- **SMS**: Kavenegar API — credentials in `appsettings.json` (`KavenegarSms`). Remove or rotate if publishing.
- **Identity**: Razor Pages identity (`/Identity/Account/Login`, `/Identity/Account/Register`).
