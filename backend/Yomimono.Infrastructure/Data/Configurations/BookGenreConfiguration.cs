using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yomimono.Domain.Entities;

namespace Yomimono.Infrastructure.Data.Configurations;

public class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
{
    public void Configure(EntityTypeBuilder<BookGenre> builder)
    {
        builder.ToTable("BookGenres");

        builder.HasKey(bg => new { bg.BookId, bg.GenreId });

        builder.HasOne(bg => bg.Book)
            .WithMany(b => b.Genres)
            .HasForeignKey(bg => bg.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bg => bg.Genre)
            .WithMany()
            .HasForeignKey(bg => bg.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
