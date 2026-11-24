using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.BLL.DTOs.Reviews;

// اللي بيتعرض للناس: مفيش فيه اسم كامل ولا أي بيانات حساب
public class ReviewDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public ReviewStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

// اللي بيتعرض للدكتور بس في شاشة المراجعة
public class ReviewModerationDto : ReviewDto
{
    public string UserFullName { get; set; } = null!;
    public DateTime? ReviewedAt { get; set; }
}

public class SaveReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
}

public class ReviewSummaryDto
{
    public List<ReviewDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public double Average { get; set; }
}