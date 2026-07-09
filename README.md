# Inventory-Management-System
A server-rendered application to manage products and categories, record stock movements (stock coming in
from suppliers, stock going out for sales/usage), automatically flag items that have fallen below a defined
reorder threshold, and show a simple report of total stock value grouped by category

## Tech Stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core (Code-First, migrations)
- SQL Server Express
- Bootstrap

## Features

- Category and Product CRUD (create, edit, delete, list)
- Record stock movements (IN for stock coming in, OUT for stock going out)
- Low stock is flagged automatically, both on a dedicated Low Stock page and directly in the main Products list
- Dashboard showing total products, total stock value, low stock count, and recent stock movements
- Stock value report grouped by category, with a grand total

## Architecture

Controller -> Service -> Repository -> Unit of Work -> DbContext -> Database

- Controllers just handle the request and return a view. No database calls in controllers.
- Services hold the business logic and map between ViewModels and entities.
- Repositories handle data access for one entity each (Category, Product, StockTransaction). They stage changes but never call SaveChanges.
- Unit of Work holds one shared DbContext, hands out the repositories, and is the only place that calls SaveChanges or controls a transaction.

Repository and Unit of Work are used together because recording a stock movement touches two entities (Product and StockTransaction) that need to be saved together. A single repository can't do that on its own, so the Unit of Work coordinates both through one shared DbContext and one transaction.

## Project Structure

```
Controllers/   - CategoriesController, ProductsController, StockTransactionController, ReportsController, HomeController
Data/          - ApplicationDbContext
Migrations/
Models/        - Category, Product, StockTransaction (with TransactionType enum)
ViewModel/     - CategoryViewModel, ProductViewModel, StockTransactionViewModel,
                 DashBoardViewModel, RecentStockTransactionViewModel,
                 StockValueReportViewModel, CategoryStockValueViewModel
Repository/    - generic IRepository/Repository, plus Category/Product/StockTransaction repositories
UoW/           - IUnitOfWork, UnitOfWork
Services/      - ServiceResult, CategoryService, ProductService, StockTransactionService
Views/         - Categories, Products, StockTransaction, Reports, Home
```

## Data Model

- **Category**: Id, Name
- **Product**: Id, Name, SKU (unique), CategoryId, UnitPrice, CurrentStockQuantity, ReorderThreshold
- **StockTransaction**: Id, ProductId, Type (IN/OUT), Quantity, Date, Note

### Why `TransactionType` Is an Enum, Not a String or Bool

- A `bool` (`IsIncoming`) cannot represent a future third transaction type (e.g., `ADJUSTMENT`) without a breaking redesign.
- A `string` allows typos/inconsistent casing with no compile-time safety.
- An `enum` gives compile-time safety, reads clearly in code (`TransactionType.IN` vs. a magic number), stores efficiently in the database as an `int`, and lets Razor auto-generate a dropdown from it.

### Why SKU Has a Unique Index

SKU is meant to be a unique product identifier. Enforcing uniqueness at the database level (`HasIndex(p => p.SKU).IsUnique()`), in addition to a proactive `SkuExistsAsync()` check in the service, ensures duplicates are impossible even under concurrent submissions — not just prevented by application logic alone.

## Design Decisions

## ServiceResult
Every service method that can fail for a business reason returns a ServiceResult instead of a bool or an exception:

```csharp
public class ServiceResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
```

The controller checks `result.Success` and shows `result.ErrorMessage` on the form if it failed.

## Atomicity (Core Requirement)

Recording a stock movement updates the Product's CurrentStockQuantity and inserts a new StockTransaction. Both need to succeed or fail together.

This is handled in `StockTransactionService.ProcessStockMovementAsync`:

```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    // update product.CurrentStockQuantity
    _unitOfWork.Products.Update(product);
    await _unitOfWork.StockTransactions.AddAsync(transaction);
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch (Exception)
{
    await _unitOfWork.RollbackAsync();
    return new ServiceResult { Success = false, ErrorMessage = "An error occurred while recording the transaction. No changes were saved." };
}
```

Both repositories are created from the same DbContext inside the Unit of Work, so one SaveChangesAsync call saves both changes together. Wrapping it in an explicit transaction means if anything fails, both changes get rolled back.

Before this runs, an OUT transaction is rejected if it would take stock below zero:

```csharp
 if (viewModel.Type == TransactionType.OUT) 
 {
     if(viewModel.Quantity > product.CurrentStockQuantity)
     {
         return new ServiceResult
         {
             Success = false,
             ErrorMessage = "Not enough stock available."
         };
     }
 }
```

I tested the rollback by forcing an exception partway through the transaction and confirming in SQL Server that neither the product's stock nor a new transaction row got saved.



## Challenges Faced

- Once I split the code into Repository and Unit of Work, the Product repository and the StockTransaction repository are two separate classes. Making sure both were still working off the same DbContext instance was important, since if they had ended up with separate DbContext instances, a single SaveChanges call would only commit one of the two changes and the atomicity requirement would silently break. The Unit of Work has to construct both repositories from the same context for the transaction to actually cover both.

- The SKU uniqueness check needed to work differently for Create versus Edit. On Create, any existing SKU match is a duplicate. On Edit, the product is allowed to keep its own current SKU, so the check has to exclude the product's own Id, otherwise every edit would incorrectly flag the SKU as already taken.

- Dropdown lists (Category list on the Product form, Product list on the Stock Movement form) had to be manually reloaded when a form failed validation, since the posted ViewModel doesn't include them.

- Didn't fully trust the atomicity was working until I forced a failure mid-transaction and checked the database directly, instead of just assuming the code was correct because it compiled.

# What I Learned

- ASP.NET Core MVC architecture
- Repository and Unit of Work patterns
- Entity Framework Core Code First
- Transaction handling
- Dependency Injection
- Service Layer architecture
- Razor Views and ViewModels

## Future Improvements

- Unit tests using xUnit, mocking IUnitOfWork so the services can be tested without a real database
- Search and filtering on the Products list
- Basic authentication & authorization
- Export the Stock Value Report to CSV
- Pagination if the product list grows large

## Running the Project

1. Update the connection string in appsettings.json for your SQL Server Express instance
2. Run Update-Database in Package Manager Console
3. Run the project (F5)
4. Use the navigation bar to reach Categories, Products, Record Stock Movement, and the Stock Value Report.