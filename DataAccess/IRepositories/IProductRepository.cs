using Models;
using StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
