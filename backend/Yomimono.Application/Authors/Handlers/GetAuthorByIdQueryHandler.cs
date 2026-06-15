using MediatR;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Authors.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Handlers;

public class GetAuthorByIdQueryHandler(IAuthorRepository repository)
    : IRequestHandler<GetAuthorByIdQuery, Result<AuthorDto>>
{
    public async Task<Result<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
            return Result<AuthorDto>.NotFound("Autor não encontrado.");

        return Result<AuthorDto>.Success(new AuthorDto(author.Id, author.Name, author.CreatedAt, author.UpdatedAt));
    }
}
