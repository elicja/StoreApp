using DataAccess.IRepositories;
using Models;
using StoreApp.DataAccess.Data;
using StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ProductImgRepository : Repository<ProductImg>, IProductImgRepository
    {
        private AppDbContext _db;

        public ProductImgRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductImg productImg)
        {
            _db.ProductImgs.Update(productImg);
        }
    }
}
