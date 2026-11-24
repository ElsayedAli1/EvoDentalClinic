using EvoDentalClinic.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Repo.InterfaceRepo
{
    public interface IUnitOfWork
    {
        IGenericRepository<Patient> Patients { get; }
        IGenericRepository<MedicalHistory> MedicalHistories { get; }
        IGenericRepository<Appointment> Appointments { get; }
        IGenericRepository<TreatmentRecord> TreatmentRecords { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<ClinicSettings> ClinicSettings { get; }
        IGenericRepository<Review> Reviews { get; }
        Task<int> SaveChangesAsync();
    }
}
