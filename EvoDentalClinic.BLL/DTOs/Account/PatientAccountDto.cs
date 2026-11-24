namespace EvoDentalClinic.BLL.DTOs.Account;

public enum AccountState
{
    None = 1,      // مفيش حساب
    Linked = 2,    // مربوط بالمريض
    Pending = 3    // حساب اتعمل برقمه ومحتاج ربط من الاستقبال
}

public class PatientAccountDto
{
    public AccountState State { get; set; } = AccountState.None;
    public string? UserName { get; set; }
    public string? Email { get; set; }
}