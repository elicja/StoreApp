using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepo { get; }
        IProductRepository ProductRepo { get; }
        ICompanyRepository CompanyRepo { get; }
        IShoppingCartRepository ShoppingCartRepo { get; }
        IAppUserRepository AppUserRepo { get; }
        IOrderDetailRepository OrderDetailRepo { get; }
        IOrderHeaderRepository OrderHeaderRepo { get; }
        IProductImgRepository ProductImgRepo { get; }

        void Save();
    }
}
