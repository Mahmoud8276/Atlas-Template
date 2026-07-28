using Atlas.Template.Core.Interfaces;
using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Atlas.Template.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // private readonly Lazy<IProductRespositoy> _productRepositoty;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

             //_productRepositoty = new Lazy<ProductRepository>(() => new ProductRepository(_context))
        }

        // public IProductRepositoty ProductRepositoty => _productRepository.Value;



        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
