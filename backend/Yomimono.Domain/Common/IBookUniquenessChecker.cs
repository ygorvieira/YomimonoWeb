namespace Yomimono.Domain.Common;

public interface IBookUniquenessChecker
{
    Task<bool> IsIsbnUniqueAsync(string isbn, Guid? excludeId, CancellationToken cancellationToken = default);
}
