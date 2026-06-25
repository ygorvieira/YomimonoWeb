using Yomimono.Domain.Entities;

namespace Yomimono.Application.Authors.Common;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Author>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
    Task AddAsync(Author entity, CancellationToken cancellationToken = default);
    void Update(Author entity);
    void Delete(Author entity);
    Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
