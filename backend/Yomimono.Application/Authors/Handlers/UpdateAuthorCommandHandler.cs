using Yomimono.Domain.Entities;
using MediatR;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Handlers;

public class UpdateAuthorCommandHandler(IAuthorRepository repository)
    : IRequestHandler<UpdateAuthorCommand, Result<AuthorDto>>
{
    public async Task<Result<AuthorDto>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
            return Result<AuthorDto>.NotFound("Autor não encontrado.");

        var duplicate = await repository.GetByNameAsync(request.Author.Name, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<AuthorDto>.Failure("Já existe um autor cadastrado com este nome.");

        var error = author.UpdateName(request.Author.Name);
        if (error is not null)
            return Result<AuthorDto>.Failure(error);

        repository.Update(author);
        return Result<AuthorDto>.Success(MapToDto(author), "Autor atualizado com sucesso.");
    }

    private static AuthorDto MapToDto(Author author) => new(author.Id, author.Name, author.CreatedAt, author.UpdatedAt);
}
