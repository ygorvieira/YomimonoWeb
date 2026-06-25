using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Authors.Common;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Repositories;

public class AuthorRepository(AppDbContext context) : IAuthorRepository
{
    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Authors
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null, cancellationToken);
    }

    public async Task<IEnumerable<Author>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = context.Authors
            .IgnoreQueryFilters()
            .Where(a => a.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(a => a.Name.Contains(searchTerm));

        return await query.OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Author entity, CancellationToken cancellationToken = default)
    {
        await context.Authors.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Author entity)
    {
        context.Authors.Update(entity);
        context.SaveChanges();
    }

    public void Delete(Author entity)
    {
        entity.Delete();
        context.Authors.Update(entity);
        context.SaveChanges();
    }

    public async Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Authors
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Name == name && a.DeletedAt == null, cancellationToken);
    }
}
