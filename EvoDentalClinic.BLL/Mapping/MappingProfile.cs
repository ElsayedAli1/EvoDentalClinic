using AutoMapper;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.BLL.DTOs.MedicalHistories;
using EvoDentalClinic.BLL.DTOs.PatientDto;
using EvoDentalClinic.BLL.DTOs.Payments;
using EvoDentalClinic.BLL.DTOs.Reviews;
using EvoDentalClinic.BLL.DTOs.Treatments;
using EvoDentalClinic.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /// Patient
            CreateMap<Patient, PatientDto>();

          
            CreateMap<CreatePatientDto, Patient>()
                .ForMember(p => p.Id, opt => opt.Ignore())
                .ForMember(p => p.Code, opt => opt.Ignore())
                .ForMember(p => p.CreatedAt, opt => opt.Ignore());

          
            CreateMap<UpdatePatientDto, Patient>()
                .ForMember(p => p.Code, opt => opt.Ignore())
                .ForMember(p => p.CreatedAt, opt => opt.Ignore());

            // Appointment
            CreateMap<Appointment, AppointmentDto>();

            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(a => a.Id, opt => opt.Ignore())
                .ForMember(a => a.Patient, opt => opt.Ignore())
                .ForMember(a => a.Status, opt => opt.Ignore())
                .ForMember(a => a.CreatedAt, opt => opt.Ignore());
            // TreatmentRecord
            CreateMap<TreatmentRecord, TreatmentRecordDto>()
                .ForMember(d => d.TotalPaid, opt => opt.MapFrom(s => s.Payments.Sum(p => p.Amount)));

            CreateMap<CreateTreatmentRecordDto, TreatmentRecord>()
                .ForMember(t => t.Id, opt => opt.Ignore())
                .ForMember(t => t.Patient, opt => opt.Ignore())
                .ForMember(t => t.Payments, opt => opt.Ignore());

            CreateMap<UpdateTreatmentRecordDto, TreatmentRecord>()
                .ForMember(t => t.PatientId, opt => opt.Ignore())
                .ForMember(t => t.Patient, opt => opt.Ignore())
                .ForMember(t => t.Payments, opt => opt.Ignore());
            // Payment
            CreateMap<Payment, PaymentDto>();

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(p => p.Id, opt => opt.Ignore())
                .ForMember(p => p.TreatmentRecord, opt => opt.Ignore())
                .ForMember(p => p.PaidAt, opt => opt.Ignore());
            // MedicalHistory
            CreateMap<MedicalHistory, MedicalHistoryDto>();

            CreateMap<MedicalHistoryDto, MedicalHistory>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Patient, opt => opt.Ignore());
            // آراء المرضى
            CreateMap<Review, ReviewDto>();
            CreateMap<Review, ReviewModerationDto>();   // UserFullName بتتاخد من User.FullName تلقائياً
        }
    }
}
