using Inventory_Management_System.Data;
using Inventory_Management_System.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory_Management_System.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public ICategoryRepository Categories { get; }
        public IProductRepository  Products { get; }
        public IStockTransactionRepository StockTransactions { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            StockTransactions = new StockTransactionRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction has been started.");
            }

            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction has been started.");
            }

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            _context.Dispose();
        }
    }
}
