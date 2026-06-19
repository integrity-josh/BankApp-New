using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data
{
    public class BankAppDbContext : DbContext
    {
        public BankAppDbContext(DbContextOptions<BankAppDbContext> options) : base(options)
        {
            // mapping the db context - we don't always need to do a lot with this, mainly we want to when we have more complex relationships

        }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankAppDbContext).Assembly); // this will apply all the configurations we have in the same assembly as the BankAppDbContext, so we can put our entity configurations in separate classes and they will be automatically applied here, which keeps our code cleaner and more organized, especially as we have more entities and more complex relationships
                // usually going to be the same assembly as the db context, but we could also use mapping files in a separate assembly if we wanted to, and just specify that assembly here instead
                // scanning for mappings on assembly happens at server startup and makes cold start a little slow, but after that it will be faster since the mappings are cached, so it's a good practice to use this approach for larger applications with more complex mappings, rather than having all the mappings in the OnModelCreating method which can get very cluttered and hard to maintain as the application grows
            
            // When your application starts up and reads Program.cs, it builds the dependency injection container and instantiates your BankAppDbContext. 
                // The very first time EF Core builds the internal database model, it runs OnModelCreating.
                // When it hits ApplyConfigurationsFromAssembly(typeof(BankAppDbContext).Assembly) in the DbContext, EF Core asks the .NET runtime:
                // "Look inside the compiled .dll file (Assembly) where BankAppDbContext lives. 
                // Find every single class that implements the IEntityTypeConfiguration<T> interface."
                    // this is how it finds and applies the CustomerMapping, AccountMapping classes, and any other mapping classes we create in the future, without us having to explicitly tell it about them

           
            // base.OnModelCreating(modelBuilder);

            // // configure the relationships between entities here, if needed
            // // for example, if we have a one-to-many relationship between Customer and Account, we can configure it here

            // modelBuilder.Entity<Customer>()
            //     .HasMany(c => c.Accounts) // a customer has many accounts
            //     .WithOne() // an account has one customer - we don't need to specify the navigation property on the account side since we're not using it, but if we were, we'd put it here
            //     .HasForeignKey("CustomerId"); // specify the foreign key in the accounts table that points to the customer - this is optional, EF will create it automatically if we don't specify it, but it's good practice to be explicit about it
        }
    }
}