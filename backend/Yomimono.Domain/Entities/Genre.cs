using Yomimono.Domain.Common;

namespace Yomimono.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; private set; } = null!;

    private Genre() { }

    public static Genre Create(string name)
    {
        return new Genre
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public string? UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "O campo nome é obrigatório.";
        if (name.Length > 100)
            return "O nome deve ter no máximo 100 caracteres.";
        Name = name;
        UpdatedAt = DateTime.UtcNow;
        return null;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
