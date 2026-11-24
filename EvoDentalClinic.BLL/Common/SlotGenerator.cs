namespace EvoDentalClinic.BLL.Common;

public static class SlotGenerator
{
    public static List<TimeOnly> Generate(TimeOnly open, TimeOnly close, int slotMinutes)
    {
        var slots = new List<TimeOnly>();
        if (slotMinutes <= 0) return slots;

        var start = open.Hour * 60 + open.Minute;
        var end = close.Hour * 60 + close.Minute;

        for (var minute = start; minute + slotMinutes <= end; minute += slotMinutes)
            slots.Add(new TimeOnly(minute / 60, minute % 60));

        return slots;
    }
}