using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yomimono.Domain.Entities;

namespace Yomimono.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Isbn)
            .HasMaxLength(20);

        builder.HasIndex(b => b.Isbn)
            .IsUnique();

        builder.Property(b => b.Publisher)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.CoverUrl)
            .HasMaxLength(500);

        builder.Property(b => b.ReadingStatus)
            .HasMaxLength(20);

        builder.Property(b => b.IsLiked)
            .HasDefaultValue(false);

        builder.Property(b => b.ReReadCount)
            .HasDefaultValue(0);

        builder.HasMany(b => b.BookAuthors)
            .WithOne(ba => ba.Book)
            .HasForeignKey(ba => ba.BookId);

        builder.HasMany(b => b.Genres)
            .WithOne(bg => bg.Book)
            .HasForeignKey(bg => bg.BookId);

    }
}
