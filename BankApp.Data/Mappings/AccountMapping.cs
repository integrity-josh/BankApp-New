using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Entities;
using BankApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApp.Data.Mappings
{
    public class AccountMapping : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts"); // map to Accounts table
            builder.HasKey(a => a.Id); // specify the primary key
            
            builder.Property(a => a.Id)
                .HasColumnName("Id"); // specify the column name for the Id property

            builder.Property(a => a.Balance)
                .HasConversion(
                    m => m.Amount,            // To DB: Extract decimal from Money object
                    d => new Money(d))        // From DB: Reconstruct Money object from decimal
                .HasColumnName("Balance"); // specify the column name for the Balance property
                // removed this - handled in Money value object now as SQLite doens't enforce this .HasColumnType("decimal(18,2)");  // enforce the 2 decimal places - however because SQLite only has Numeric, this may not work quite right

            // could set up one to many relationship with customer here but not needed - already done in customer, could be done in both or just in accounts, but personal preference just doing it in customermapping
        }
    }
}