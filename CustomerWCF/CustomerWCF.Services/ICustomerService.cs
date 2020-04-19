using CustomerWCF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CustomerWCF.Services
{
    [ServiceContract]
    public interface ICustomerService
    {
        [OperationContract]
        List<Product> GetProducts();

        [OperationContract]
        List<Customer> GetCustomers();

        [OperationContract]
        void SubmitOrder(Order order);
    }
}
