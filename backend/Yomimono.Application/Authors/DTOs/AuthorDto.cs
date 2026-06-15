namespace Yomimono.Application.Authors.DTOs;

public record AuthorDto(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedAt);
public record CreateAuthorDto(string Name);
public record UpdateAuthorDto(string Name);
