using MediatR;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Authors.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Handlers;

public class GetAllAuthorsQueryHandler(IAuthorRepository repository)
    : IRequestHandler<GetAllAuthorsQuery, Result<IEnumerable<AuthorDto>>>
{
    public async Task<Result<IEnumerable<AuthorDto>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await repository.GetAllAsync(request.SearchTerm, cancellationToken);
        var dtos = authors.Select(a => new AuthorDto(a.Id, a.Name, a.CreatedAt, a.UpdatedAt));
        return Result<IEnumerable<AuthorDto>>.Success(dtos);
    }
}
