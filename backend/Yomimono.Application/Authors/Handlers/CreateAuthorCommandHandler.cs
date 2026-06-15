using MediatR;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Application.Authors.Handlers;

public class CreateAuthorCommandHandler(IAuthorRepository repository)
    : IRequestHandler<CreateAuthorCommand, Result<AuthorDto>>
{
    public async Task<Result<AuthorDto>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var duplicate = await repository.GetByNameAsync(request.Author.Name, cancellationToken);
        if (duplicate is not null)
            return Result<AuthorDto>.Failure("Já existe um autor cadastrado com este nome.");

        var (author, error) = Author.Create(request.Author.Name);
        if (error is not null)
            return Result<AuthorDto>.Failure(error);

        await repository.AddAsync(author!, cancellationToken);
        return Result<AuthorDto>.Created(MapToDto(author!), "Autor cadastrado com sucesso.");
    }

    private static AuthorDto MapToDto(Author author) => new(author.Id, author.Name, author.CreatedAt, author.UpdatedAt);
}
