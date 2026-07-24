using System;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    /// <summary>
    /// Enterprise Unit of Work
    /// Abstraction for transaction management.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();

        Task<int> SaveChangesAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}