using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Entities;
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
                .HasColumnName("Balance") // specify the column name for the Balance property
                .HasColumnType("decimal(18,2)");  // enforce the 2 decimal places - however because SQLite only has Numeric, this may not work quite right

            // could set up one to many relationship with customer here but not needed - already done in customer, could be done in both or just in accounts, but personal preference just doing it in customermapping
        }
    }
}