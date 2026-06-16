using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Books.Common;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Repositories;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Books
            .IgnoreQueryFilters()
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.Genres).ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(Guid? genreId = null, Guid? authorId = null, string? readingStatus = null, CancellationToken cancellationToken = default)
    {
        var query = context.Books
            .IgnoreQueryFilters()
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.Genres).ThenInclude(bg => bg.Genre)
            .Where(b => b.DeletedAt == null);

        if (genreId.HasValue)
            query = query.Where(b => b.Genres.Any(bg => bg.GenreId == genreId.Value));

        if (authorId.HasValue)
            query = query.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId.Value));

        if (!string.IsNullOrWhiteSpace(readingStatus))
            query = query.Where(b => b.ReadingStatus == readingStatus);

        return await query.OrderBy(b => b.Title).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Book entity, CancellationToken cancellationToken = default)
    {
        await context.Books.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Book entity)
    {
        context.Books.Update(entity);
        context.SaveChanges();
    }

    public void Delete(Book entity)
    {
        entity.Delete();
        context.Books.Update(entity);
        context.SaveChanges();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await context.Books
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Isbn == isbn && b.DeletedAt == null, cancellationToken);
    }
}
