using System;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //DanhMucController

        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        void Commit();
    }
}
