namespace EvoDentalClinic.PL.ViewModels;

public class BookAppointmentViewModel
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public string PatientPhone { get; set; } = null!;
    public DateOnly Date { get; set; }
    public DateOnly MinDate { get; set; }
    public List<TimeOnly> Slots { get; set; } = new();
}