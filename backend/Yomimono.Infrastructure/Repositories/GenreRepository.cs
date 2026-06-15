using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Genres.Common;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Repositories;

public class GenreRepository(AppDbContext context) : IGenreRepository
{
    public async Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Genres
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null, cancellationToken);
    }

    public async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Genres
            .IgnoreQueryFilters()
            .Where(g => g.DeletedAt == null)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Genre entity, CancellationToken cancellationToken = default)
    {
        await context.Genres.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Genre entity)
    {
        context.Genres.Update(entity);
        context.SaveChanges();
    }

    public void Delete(Genre entity)
    {
        entity.Delete();
        context.Genres.Update(entity);
        context.SaveChanges();
    }

    public async Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Genres
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Name == name && g.DeletedAt == null, cancellationToken);
    }
}
