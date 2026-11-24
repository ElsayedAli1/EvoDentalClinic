using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.DAL.Entities;

public class Review
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public string DisplayName { get; set; } = null!;   // الاسم الأول بس
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;

    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}