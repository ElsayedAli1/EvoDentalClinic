using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            
            builder.Property(a => a.Notes).HasMaxLength(500);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

          
            builder.HasIndex(a => new { a.Date, a.StartTime })
                   .IsUnique()
                   .HasFilter($"[Status] <> {(int)AppointmentStatus.Cancelled}");
        }
    }
}
