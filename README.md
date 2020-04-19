# customerapp-wcf

Step by step tutorials creating WCF applications

## Table of Contents

- [Sending Feedback](#sending-feedback)
- [About WCF](#about-entity-framework-core)
- [Sample application with each labs](#sample-application-with-each-steps)
    - Creating WCF application
        - [Step 1 - Create Application](#step-1---create-application)
    - Implementing Services
        - [Step 2 - Adding Data contract](#step-2---adding-data-contract)
        - [Step 3 - Adding Service contract and Operation contract in interface](#step-3---adding-service-contract-and-operation-contract-in-interface)
        - [Step 4 - Adding EntityFramework and DbContext](#step-4---adding-entityframework-and-dbcontext)
        - [Step 5 - Service Implementation](#step-5---service-implementation)
        
## Sending Feedback

For feedback can drop mail to my email address amit.naik8103@gmail.com or you can create [issue](https://github.com/Amitpnk/angular-application/issues/new)

## About WCF

* Windows Communication Foundation 
    * Basically it is platform for building distributed service-orieted application
        * Defines services and host for those services
        * Defines clients to connect to services
* SOAP Messaging
    * It is XML based protocol information at a wire-level
    * WCF is messaging system 

## Implementing Services

### Step 1 - Create Application

* Create Blank solution CustomerWCF
* Class WCF Service library 
    * CustomerWCF.Services
* Delete IService1.cs and Service1.cs file as we are going to create new
* Delete below code snippet from App.config file, which is no valid as we deleted IService1.cs file

```xml
<service name="CustomerWCF.Services.Service1">
<!-- includ -->
</service>
```
* Go to properties -> WCF Options
    * Uncheck startup option <b>Start WCF service Host when debugging another project in the same solution</b>
* Go to properties -> Debug 
    * Comment command line arguements in  Start options

```sql
--/client:"WcfTestClient.exe"
```

Now it will be true class library with 2 library
* System.ServiceModel (Uses when we do WCF stuff)
* System.Runtime.Serialization (Uses when we do Data contract)

### Step 2 - Adding Data contract

* Add class library 
    * CustomerWCF.Domain
* Create Entities as
    * Customer
    * Order
    * OrderItem
    * OrderStatus
    * Product

* Data contract is a formal agreement between a service and a client that abstractly describes the data to be exchanged.
* Data member are the fields or properties of your Data Contract class

```c#
[DataContract]
public class Customer
{
    [DataMember]
    public Guid Id { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public string Phone { get; set; }
    [DataMember]
    public string Email { get; set; }
    [DataMember]
    public string Street { get; set; }
    [DataMember]
    public string City { get; set; }
    [DataMember]
    public string State { get; set; }
    [DataMember]
    public string Zip { get; set; }
    // Need to have set block or will get obscure error during serialization
    [DataMember]
    public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } set { } }
}

[DataContract]
public class Order
{
    public Order()
    {
        OrderItems = new List<OrderItem>();
    }
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public Guid CustomerId { get; set; }
    [DataMember]
    public int OrderStatusId { get; set; }
    [DataMember]
    public DateTime OrderDate { get; set; }
    [DataMember]
    public decimal ItemsTotal { get; set; }
    [DataMember]
    public List<OrderItem> OrderItems { get; set; }
}

[DataContract]
public partial class OrderItem
{
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public long OrderId { get; set; }
    [DataMember]
    public int ProductId { get; set; }
    [DataMember]
    public int Quantity { get; set; }
    [DataMember]
    public decimal UnitPrice { get; set; }
    [DataMember]
    public decimal TotalPrice { get; set; }
}

public class OrderStatus
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[DataContract]
public class Product
{
    [DataMember]
    public int Id { get; set; }
    [DataMember]
    public string Type { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Description { get; set; }
}
```

### Step 3 - Adding Service contract and Operation contract in interface

* Service contract is interface of WC service, which are exposed by service to outside world.
* Operation contract is attribute to define methods inside servcie contract. OperationContract methods are exposed to client

```c#
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
```

## Step 4 - Adding EntityFramework and DbContext

* Add class library 
    * CustomerWCF.Data
* Create Entities as CustomerDBContext
* Add EntityFramework via nuget package manager

```C#
public class CustomerDBContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
}
```

* In Package Manager console run below command, under <b>CustomerWCF.Data</b>
    1. enable-migrations (this will create configuration)

    * Create constructor and add connection string

    ```C#
    public class CustomerDBContext : DbContext
    {
        //...
        public CustomerDBContext() : base("Data Source=(local)\\SQLexpress;Initial Catalog=CustomerWCF;Integrated Security=True")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CustomerDBContext, Configuration>());
        }
    }
    ```

    2. add-migration initial-setup
    3. update-database

## Step 5 - Service Implementation

* ServiceBehavior - execution behavior of a service contract implementation class
    * PerCall - New service object is created on each call
    * PerSession -  New service instance for each new session (This will be default)
    * Single - One instance of service for all calls
* OperationBehavior - execution behavior of a service operation
    * TransactionScopeRequired - To set transaction for method

```c#
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
```