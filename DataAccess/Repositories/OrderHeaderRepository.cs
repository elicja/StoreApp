using DataAccess.IRepositories;
using Models;
using Newtonsoft.Json.Bson;
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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _db;

        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);

            if (orderFromDb != null)
            {
				orderFromDb.OrderStatus = orderStatus;

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentId)
        {
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);

			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
			}

			if (!string.IsNullOrEmpty(paymentId))
			{
				orderFromDb.PaymentId = paymentId;
				orderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
