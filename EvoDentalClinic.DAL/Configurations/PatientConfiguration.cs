using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            
            builder.Property(p => p.Code).IsRequired().HasMaxLength(20);
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(15);
            builder.Property(p => p.Address).HasMaxLength(250);
            builder.Property(p => p.Job).HasMaxLength(100);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            
            builder.HasIndex(p => p.Phone).IsUnique();
            builder.HasIndex(p => p.Code).IsUnique();

           
            builder.HasOne(p => p.User)
                   .WithOne()
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.MedicalHistory)
                   .WithOne(m => m.Patient)
                   .HasForeignKey<MedicalHistory>(m => m.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Patient)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.TreatmentRecords)
                   .WithOne(t => t.Patient)
                   .HasForeignKey(t => t.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
