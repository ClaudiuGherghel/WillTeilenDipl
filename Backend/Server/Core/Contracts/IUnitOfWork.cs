using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        IRentalRepository RentalRepository { get; }
        IImageRepository ImageRepository { get; }
        IItemRepository ItemRepository { get; }
        IUserRepository UserRepository { get; }

        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task<int> SaveChangesAsync();
        Task FillDbAsync();

    }
}
