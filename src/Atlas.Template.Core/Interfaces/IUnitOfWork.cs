using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Atlas.Template.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        // Ex:
        //public IProductRepositoty ProductRepositoty { get; }

        public Task CompleteAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
