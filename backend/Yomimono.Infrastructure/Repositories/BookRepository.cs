using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Books.Common;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Repositories;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Books.OrderBy(b => b.Title).ToListAsync(cancellationToken);
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

    public async Task<IEnumerable<Book>> GetByGenreAsync(string genre, CancellationToken cancellationToken = default)
    {
        return await context.Books
            .Where(b => b.Genre == genre)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await context.Books.FirstOrDefaultAsync(b => b.Isbn == isbn, cancellationToken);
    }
}
