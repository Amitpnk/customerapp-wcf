using CustomerWCF.Data;
using CustomerWCF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CustomerWCF.Services
{
    [ServiceBehavior(InstanceContextMode =InstanceContextMode.PerCall)]
    public class CustomerService : ICustomerService, IDisposable
    {
        private CustomerDBContext context = new CustomerDBContext();

        // Dispose method will call by wcf when it is done
        public void Dispose()
        {
            context.Dispose();
        }

        public List<Customer> GetCustomers()
        {
            return context.Customers.ToList();
        }

        public List<Product> GetProducts()
        {
            return context.Products.ToList();
        }

        public void SubmitOrder(Order order)
        {
            context.Orders.Add(order);
            order.OrderItems.ForEach(o => context.OrderItems.Add(o));
            context.SaveChanges();
        }
    }
}
