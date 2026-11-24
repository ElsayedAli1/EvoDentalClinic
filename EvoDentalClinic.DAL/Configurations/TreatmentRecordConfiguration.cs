using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{
    public class TreatmentRecordConfiguration : IEntityTypeConfiguration<TreatmentRecord>
    {
        public void Configure(EntityTypeBuilder<TreatmentRecord> builder)
        {
           
            builder.Property(t => t.Procedure).IsRequired().HasMaxLength(200);
            builder.Property(t => t.TotalCost).HasPrecision(18, 2);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TreatmentRecord_TotalCost", "[TotalCost] >= 0");
                t.HasCheckConstraint("CK_TreatmentRecord_ToothNumber", "[ToothNumber] BETWEEN 11 AND 85");
            });

           
            builder.HasMany(t => t.Payments)
                   .WithOne(p => p.TreatmentRecord)
                   .HasForeignKey(p => p.TreatmentRecordId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
