using Microsoft.EntityFrameworkCore;
using Yomimono.Domain.Common;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Repositories;

public class BookUniquenessChecker(AppDbContext context) : IBookUniquenessChecker
{
    public async Task<bool> IsIsbnUniqueAsync(string isbn, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        var query = context.Books.Where(b => b.Isbn == isbn);

        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }
}
