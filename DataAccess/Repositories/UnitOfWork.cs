﻿using DataAccess.IRepositories;
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

        public UnitOfWork(AppDbContext db)
        {
            _db = db;

            CategoryRepo = new CategoryRepository(_db);
            ProductRepo = new ProductRepository(_db);
            CompanyRepo = new CompanyRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
