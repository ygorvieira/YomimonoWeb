using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yomimono.Domain.Entities;

namespace Yomimono.Infrastructure.Data.Configurations;

public class BookEditionConfiguration : IEntityTypeConfiguration<BookEdition>
{
    public void Configure(EntityTypeBuilder<BookEdition> builder)
    {
        builder.ToTable("BookEditions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Number)
            .IsRequired();

        builder.HasOne(e => e.Book)
            .WithMany(b => b.BookEditions)
            .HasForeignKey(e => e.BookId);
    }
}
