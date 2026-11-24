using EvoDentalClinic.BLL.DTOs.Appointments;

namespace EvoDentalClinic.PL.ViewModels;

public class PortalIndexViewModel
{
    public string PatientName { get; set; } = null!;
    public List<AppointmentDto> Upcoming { get; set; } = new();
    public List<AppointmentDto> Past { get; set; } = new();
}