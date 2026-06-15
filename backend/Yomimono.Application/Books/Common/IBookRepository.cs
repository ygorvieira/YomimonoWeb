using Yomimono.Domain.Entities;

namespace Yomimono.Application.Books.Common;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(Guid? genreId = null, Guid? authorId = null, string? readingStatus = null, CancellationToken cancellationToken = default);
    Task AddAsync(Book entity, CancellationToken cancellationToken = default);
    void Update(Book entity);
    void Delete(Book entity);
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}
