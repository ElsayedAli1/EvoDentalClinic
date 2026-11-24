using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{

    public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
    {
        public void Configure(EntityTypeBuilder<MedicalHistory> builder)
        {
            builder.Property(m => m.DrugAllergyType).HasMaxLength(200);
            builder.Property(m => m.RecentVisitReason).HasMaxLength(250);
            builder.Property(m => m.ProstheticsType).HasMaxLength(200);
            builder.Property(m => m.Notes).HasMaxLength(1000);
        }
    }
}
