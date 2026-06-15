namespace Yomimono.Application.Genres.DTOs;

public record GenreDto(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedAt);
public record CreateGenreDto(string Name);
public record UpdateGenreDto(string Name);
