using EvoDentalClinic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoDentalClinic.DAL.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Comment).IsRequired().HasMaxLength(600);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5"));

        // رأي واحد لكل حساب
        builder.HasIndex(r => r.UserId).IsUnique();

        // استعلام سريع عن الآراء حسب الحالة
        builder.HasIndex(r => new { r.Status, r.CreatedAt });

        // لو الحساب اتمسح، رأيه يتمسح معاه
        builder.HasOne(r => r.User)
               .WithMany()
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}