using System.Text.Json.Serialization;

namespace Yomimono.Application.Common;

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPrevPage
);
