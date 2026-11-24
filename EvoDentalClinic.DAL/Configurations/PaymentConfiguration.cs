using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Amount).HasPrecision(18, 2);
            builder.Property(p => p.Note).HasMaxLength(250);
            builder.Property(p => p.PaidAt).HasDefaultValueSql("GETUTCDATE()");

            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Payment_Amount", "[Amount] > 0"));
        }
    }
}