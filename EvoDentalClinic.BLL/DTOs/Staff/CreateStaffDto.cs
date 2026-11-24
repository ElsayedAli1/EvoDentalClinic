namespace EvoDentalClinic.BLL.DTOs.Staff;

public class CreateStaffDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}