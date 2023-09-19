using DataAccess.IRepositories;
using StoreApp.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public ICategoryRepository CategoryRepo { get; private set; }
        public IProductRepository ProductRepo { get; private set; }
        public ICompanyRepository CompanyRepo { get; private set; }
        public IShoppingCartRepository ShoppingCartRepo { get; private set; }
        public IAppUserRepository AppUserRepo { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepo { get; private set; }
        public IOrderDetailRepository OrderDetailRepo { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;

            CategoryRepo = new CategoryRepository(_db);
            ProductRepo = new ProductRepository(_db);
            CompanyRepo = new CompanyRepository(_db);
            ShoppingCartRepo = new ShoppingCartRepository(_db);
            AppUserRepo = new AppUserRepository(_db);
            OrderHeaderRepo = new OrderHeaderRepository(_db);
            OrderDetailRepo = new OrderDetailRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
