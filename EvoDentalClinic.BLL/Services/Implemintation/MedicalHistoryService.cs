using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.MedicalHistories;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class MedicalHistoryService : IMedicalHistoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MedicalHistoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<MedicalHistoryDto?> GetByPatientAsync(int patientId)
    {
        var history = await FindAsync(patientId);
        return history is null ? null : _mapper.Map<MedicalHistoryDto>(history);
    }

    public async Task<Result> SaveAsync(MedicalHistoryDto dto)
    {
        var patient = await _uow.Patients.GetByIdAsync(dto.PatientId);
        if (patient is null)
            return Result.Failure("المريض غير موجود");

        if (patient.Gender != Gender.Female && (dto.IsPregnant || dto.IsBreastfeeding))
            return Result.Failure("الحمل والرضاعة ينطبقوا على المريضات بس");

        Clean(dto);

        var error = ValidateLengths(dto);
        if (error is not null)
            return Result.Failure(error);

        var existing = await FindAsync(dto.PatientId);

        if (existing is null)
        {
            await _uow.MedicalHistories.AddAsync(_mapper.Map<MedicalHistory>(dto));
        }
        else
        {
            _mapper.Map(dto, existing);
            _uow.MedicalHistories.Update(existing);
        }

        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    // ---------- دوال مساعدة ----------

    private async Task<MedicalHistory?> FindAsync(int patientId)
    {
        var list = await _uow.MedicalHistories.FindAsync(m => m.PatientId == patientId);
        return list.FirstOrDefault();
    }

    // لو الإجابة "لا"، نمسح التفصيلة اللي تابعة لها
    private static void Clean(MedicalHistoryDto dto)
    {
        dto.DrugAllergyType = dto.HasDrugAllergy ? Normalize(dto.DrugAllergyType) : null;
        dto.RecentVisitReason = dto.HadRecentDentalVisit ? Normalize(dto.RecentVisitReason) : null;
        dto.ProstheticsType = dto.HadPreviousProsthetics ? Normalize(dto.ProstheticsType) : null;

        if (!dto.HadPreviousExtraction)
        {
            dto.HadAnestheticAllergy = false;
            dto.HadDelayedHealing = false;
            dto.HadBleeding = false;
            dto.HadFainting = false;
        }

        dto.Notes = Normalize(dto.Notes);
    }

    private static string? Normalize(string? text)
        => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    private static string? ValidateLengths(MedicalHistoryDto dto)
    {
        if (dto.DrugAllergyType?.Length > 200) return "نوع الحساسية طويل جداً (الحد الأقصى 200 حرف)";
        if (dto.RecentVisitReason?.Length > 250) return "سبب الزيارة طويل جداً (الحد الأقصى 250 حرف)";
        if (dto.ProstheticsType?.Length > 200) return "نوع التركيب طويل جداً (الحد الأقصى 200 حرف)";
        if (dto.Notes?.Length > 1000) return "الملاحظات طويلة جداً (الحد الأقصى 1000 حرف)";
        return null;
    }
}