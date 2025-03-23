using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Repositories;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopQuanAoEntities _context;
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;

        public UnitOfWork(ShopQuanAoEntities context)
        {
            _context = context;
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_context);
                }
                return _categoryRepository;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}