using EvoDentalClinic.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Context
{
   public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<TreatmentRecord> TreatmentRecords => Set<TreatmentRecord>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<ClinicSettings> ClinicSettings => Set<ClinicSettings>();
        public DbSet<Review> Reviews => Set<Review>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
