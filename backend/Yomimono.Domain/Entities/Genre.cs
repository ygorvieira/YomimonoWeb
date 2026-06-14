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
}
