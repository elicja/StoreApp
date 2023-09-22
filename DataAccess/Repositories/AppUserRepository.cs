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
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private AppDbContext _db;

        public AppUserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AppUser appUser)
        {
            _db.AppUsers.Update(appUser);
        }
    }
}
