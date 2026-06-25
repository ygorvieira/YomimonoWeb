using Yomimono.Application.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Application.Books.Common;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(Guid? genreId = null, Guid? authorId = null, string? readingStatus = null, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> GetAllPagedAsync(Guid? genreId, Guid? authorId, string? readingStatus, string? searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Book entity, CancellationToken cancellationToken = default);
    void Update(Book entity);
    void Delete(Book entity);
    Task<IEnumerable<Book>> GetAllForReportsAsync(CancellationToken cancellationToken = default);
}
