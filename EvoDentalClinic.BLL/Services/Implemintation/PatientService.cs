using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.PatientDto;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using System.Linq.Expressions;
using System.Text.RegularExpressions;


namespace EvoDentalClinic.BLL.Services.Implemintation
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PatientDto?> GetByIdAsync(int id)
        {
            var patient = await _uow.Patients.GetByIdAsync(id);
            return patient is null ? null : _mapper.Map<PatientDto>(patient);
        }

        public async Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 5, 100);

            Expression<Func<Patient, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(term))
            {
                var t = term.Trim();
                filter = p => p.Phone.Contains(t) || p.Code.Contains(t) || p.FullName.Contains(t);
            }

            // الأحدث الأول
            var (items, total) = await _uow.Patients.GetPagedAsync(
                filter, page, pageSize, p => p.Id, descending: true);

            return new PagedResult<PatientDto>
            {
                Items = _mapper.Map<List<PatientDto>>(items),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }
        public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto)
        {
            var phone = PhoneHelper.Normalize(dto.Phone);
            if (phone is null)
                return Result<PatientDto>.Failure("رقم الموبايل غير صحيح");

            if (IsFutureDate(dto.DateOfBirth))
                return Result<PatientDto>.Failure("تاريخ الميلاد لا يمكن أن يكون في المستقبل");

            if (await _uow.Patients.AnyAsync(p => p.Phone == phone))
                return Result<PatientDto>.Failure("رقم الموبايل ده مسجل لمريض تاني");

            var patient = _mapper.Map<Patient>(dto);
            patient.Phone = phone;
            patient.Code = Guid.NewGuid().ToString("N")[..20]; // كود مؤقت

            await _uow.Patients.AddAsync(patient);
            await _uow.SaveChangesAsync();                     // هنا الـ Id اتولّد

            patient.Code = $"P-{patient.Id:D5}";               // الكود النهائي
            await _uow.SaveChangesAsync();

            return Result<PatientDto>.Success(_mapper.Map<PatientDto>(patient));
        }

        public async Task<Result> UpdateAsync(UpdatePatientDto dto)
        {
            var patient = await _uow.Patients.GetByIdAsync(dto.Id);
            if (patient is null)
                return Result.Failure("المريض غير موجود");

            var phone = NormalizePhone(dto.Phone);
            if (phone is null)
                return Result.Failure("رقم الموبايل غير صحيح");
            if (patient.UserId is not null && patient.Phone != phone)
                return Result.Failure("المريض ده عنده حساب أونلاين برقمه، مينفعش تغيّر الرقم. احذف الحساب الأول لو محتاج");
            if (IsFutureDate(dto.DateOfBirth))
                return Result.Failure("تاريخ الميلاد لا يمكن أن يكون في المستقبل");

            if (await _uow.Patients.AnyAsync(p => p.Phone == phone && p.Id != dto.Id))
                return Result.Failure("رقم الموبايل ده مسجل لمريض تاني");

            _mapper.Map(dto, patient);
            patient.Phone = phone;

            _uow.Patients.Update(patient);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        

        private static string? NormalizePhone(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var digits = Regex.Replace(input, @"[\s\-]", "");

            if (digits.StartsWith("+20")) digits = "0" + digits[3..];
            else if (digits.StartsWith("0020")) digits = "0" + digits[4..];

            return Regex.IsMatch(digits, @"^01[0125]\d{8}$") ? digits : null;
        }

        private static bool IsFutureDate(DateOnly? date)
            => date is not null && date > DateOnly.FromDateTime(DateTime.Today);
        public async Task<PatientDto?> GetByUserIdAsync(string userId)
        {
            var list = await _uow.Patients.FindAsync(p => p.UserId == userId);
            var patient = list.FirstOrDefault();
            return patient is null ? null : _mapper.Map<PatientDto>(patient);
        }
    }
}
