using CustomerWCF.Data.Migrations;
using CustomerWCF.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerWCF.Data
{
    public class CustomerDBContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public CustomerDBContext() : base("Data Source=(local)\\SQLexpress;Initial Catalog=CustomerWCF;Integrated Security=True")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CustomerDBContext, Configuration>());
        }

    }
}
