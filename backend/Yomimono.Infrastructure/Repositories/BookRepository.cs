using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Common;
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
            .Include(b => b.BookEditions)
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(Guid? genreId = null, Guid? authorId = null, string? readingStatus = null, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = BuildFilteredQuery(genreId, authorId, readingStatus, searchTerm);
        return await query.OrderBy(b => b.Title).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Book>> GetAllPagedAsync(Guid? genreId, Guid? authorId, string? readingStatus, string? searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = BuildFilteredQuery(genreId, authorId, readingStatus, searchTerm);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<Book>(
            items,
            totalCount,
            pageNumber,
            pageSize,
            totalPages,
            HasNextPage: pageNumber < totalPages,
            HasPrevPage: pageNumber > 1
        );
    }

    public async Task AddAsync(Book entity, CancellationToken cancellationToken = default)
    {
        await context.Books.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Book entity)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        await context.BookEditions.Where(e => e.BookId == entity.Id).ExecuteDeleteAsync();
        await context.Set<BookAuthor>().Where(ba => ba.BookId == entity.Id).ExecuteDeleteAsync();
        await context.BookGenres.Where(bg => bg.BookId == entity.Id).ExecuteDeleteAsync();

        context.ChangeTracker.Clear();

        context.Attach(entity).State = EntityState.Modified;

        foreach (var author in entity.BookAuthors)
            context.Entry(author).State = EntityState.Added;
        foreach (var genre in entity.Genres)
            context.Entry(genre).State = EntityState.Added;
        foreach (var edition in entity.BookEditions)
            context.Entry(edition).State = EntityState.Added;

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public void Delete(Book entity)
    {
        entity.Delete();
        context.Books.Update(entity);
        context.SaveChanges();
    }

    public async Task<IEnumerable<Book>> GetAllForReportsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Books
            .IgnoreQueryFilters()
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.Genres).ThenInclude(bg => bg.Genre)
            .Where(b => b.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Book> BuildFilteredQuery(Guid? genreId, Guid? authorId, string? readingStatus, string? searchTerm = null)
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

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b => b.Title.Contains(searchTerm));

        return query;
    }
}
