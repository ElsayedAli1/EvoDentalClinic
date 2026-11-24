using EvoDentalClinic.BLL.DTOs.Reviews;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.PL.ViewModels;

public class ReviewModerationViewModel
{
    public ReviewStatus Status { get; set; }
    public List<ReviewModerationDto> Items { get; set; } = new();
    public int PendingCount { get; set; }
}