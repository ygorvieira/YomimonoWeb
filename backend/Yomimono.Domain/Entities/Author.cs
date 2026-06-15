using Yomimono.Domain.Common;

namespace Yomimono.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; private set; } = null!;

    private Author() { }

    public static (Author? author, string? error) Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (null, "O campo nome é obrigatório.");
        if (name.Length > 150)
            return (null, "O nome deve ter no máximo 150 caracteres.");

        return (new Author
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }, null);
    }

    public string? UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "O campo nome é obrigatório.";
        if (name.Length > 150)
            return "O nome deve ter no máximo 150 caracteres.";
        Name = name;
        UpdatedAt = DateTime.UtcNow;
        return null;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
