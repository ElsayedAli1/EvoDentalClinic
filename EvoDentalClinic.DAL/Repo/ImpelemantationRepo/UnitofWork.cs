using EvoDentalClinic.DAL.Context;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Repo.ImpelemantationRepo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IGenericRepository<Patient>? _patients;
        private IGenericRepository<MedicalHistory>? _medicalHistories;
        private IGenericRepository<Appointment>? _appointments;
        private IGenericRepository<TreatmentRecord>? _treatmentRecords;
        private IGenericRepository<Payment>? _payments;
        private IGenericRepository<ClinicSettings>? _clinicSettings;
        private IGenericRepository<Review>? _reviews;

        public IGenericRepository<Review> Reviews
            => _reviews ??= new GenericRepository<Review>(_context);
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Patient> Patients
            => _patients ??= new GenericRepository<Patient>(_context);

        public IGenericRepository<MedicalHistory> MedicalHistories
            => _medicalHistories ??= new GenericRepository<MedicalHistory>(_context);

        public IGenericRepository<Appointment> Appointments
            => _appointments ??= new GenericRepository<Appointment>(_context);

        public IGenericRepository<TreatmentRecord> TreatmentRecords
            => _treatmentRecords ??= new GenericRepository<TreatmentRecord>(_context);

        public IGenericRepository<Payment> Payments
            => _payments ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<ClinicSettings> ClinicSettings
            => _clinicSettings ??= new GenericRepository<ClinicSettings>(_context);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
