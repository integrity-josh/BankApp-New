using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApp.Data.Mappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // map explicitly to not rely on default conventions
            // not necessary but good practice to be explicit about it, especially as the application grows and we have more complex relationships
            builder.ToTable("Customers"); // map to Customers table - used to map the domain layer to the database, as they don't need to have the same names when you use this- this is optional, EF will create the table with the name of the entity by default, but it's good practice to be explicit about it
            builder.HasKey(c => c.Id); // specify the primary key - this is optional, EF will assume Id is the primary key by convention, but it's good practice to be explicit about it
            
            builder.Property(c => c.Id)
                .HasColumnName("Id"); // specify the column name for the Id property - this is optional, EF will use the property name as the column name by default, but it's good practice to be explicit about it
            // builder.Property(c => c.Id)
            //     .HasColumnName('FirstName'); // do this later when it's in the model .. rn it's just in the DB

            builder.OwnsOne(c => c.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("FirstName") // specify the column name for the FirstName property of the Name value object
                    .HasMaxLength(25) // enforce a max length of 100 characters for the FirstName column - this is optional, but it's good practice to enforce max lengths for string columns to prevent issues with data truncation and to improve performance
                    .IsRequired();
                name.Property(n => n.LastName)
                    .HasColumnName("LastName") // specify the column name for the LastName property of the Name value object
                    .HasMaxLength(25) // enforce a max length of 100 characters for the LastName column - this is optional, but it's good practice to enforce max lengths for string columns to prevent issues with data truncation and to improve performance
                    .IsRequired();
            });

            builder.HasMany(c => c.Accounts) // a customer has many accounts
                .WithOne(); // so EF knows it's a one to many relationship - could instead map this one to many relationship with the Accounts mapping, but it's better to map it with the owner (if I delete the account, the customer is still here)
                // all of this EF would know by default by convention, but it's good practice to be explicit about it

                

        }

    }
}