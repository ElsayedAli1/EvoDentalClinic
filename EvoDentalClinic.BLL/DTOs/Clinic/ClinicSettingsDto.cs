using EvoDentalClinic.BLL.Common;

namespace EvoDentalClinic.BLL.DTOs.Clinic;

public class ClinicSettingsDto
{
    public List<DayOfWeek> WorkDays { get; set; } = new();
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public int SlotMinutes { get; set; }
    public int SlotsPerDay => SlotGenerator.Generate(OpenTime, CloseTime, SlotMinutes).Count;
}