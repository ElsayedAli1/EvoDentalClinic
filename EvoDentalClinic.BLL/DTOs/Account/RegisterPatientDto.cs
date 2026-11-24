using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.BLL.DTOs.Account;

public class RegisterPatientDto
{
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
    public Gender Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public enum RegistrationOutcome
{
    Active = 1,        // اتعمل ملف مريض مربوط بالحساب
    PendingLink = 2    // الرقم مسجل في العيادة، محتاج ربط من الاستقبال
}