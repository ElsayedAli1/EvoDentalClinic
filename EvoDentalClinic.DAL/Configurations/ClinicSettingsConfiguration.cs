using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{
    public class ClinicSettingsConfiguration : IEntityTypeConfiguration<ClinicSettings>
    {
        public void Configure(EntityTypeBuilder<ClinicSettings> builder)
        {
           
            builder.Property(s => s.WorkDays).IsRequired().HasMaxLength(100);

           
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ClinicSettings_SlotMinutes", "[SlotMinutes] > 0");
                t.HasCheckConstraint("CK_ClinicSettings_Hours", "[CloseTime] > [OpenTime]");
            });

            
            builder.HasData(new ClinicSettings
            {
                Id = 1,
                WorkDays = "Saturday,Sunday,Monday,Tuesday,Wednesday,Thursday",
                OpenTime = new TimeOnly(16, 0),
                CloseTime = new TimeOnly(21, 0),
                SlotMinutes = 30
            });
        }
    }
}
