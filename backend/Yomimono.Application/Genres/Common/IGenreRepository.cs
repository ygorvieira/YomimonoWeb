using Yomimono.Domain.Entities;

namespace Yomimono.Application.Genres.Common;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Genre>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
    Task AddAsync(Genre entity, CancellationToken cancellationToken = default);
    void Update(Genre entity);
    void Delete(Genre entity);
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
