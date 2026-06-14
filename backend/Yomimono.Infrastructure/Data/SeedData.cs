using Microsoft.EntityFrameworkCore;
using Yomimono.Domain.Entities;

namespace Yomimono.Infrastructure.Data;

public static class SeedData
{
    private static readonly string[] DefaultGenres =
    [
        "Política",
        "Filosofia",
        "Ocultismo",
        "Ficção",
        "Fantasia",
        "Crônicas",
        "Quadrinhos",
        "Idiomas",
        "Videogames",
        "Tecnologia"
    ];

    public static async Task SeedGenresAsync(AppDbContext dbContext)
    {
        if (await dbContext.Genres.AnyAsync())
            return;

        var genres = DefaultGenres.Select(Genre.Create).ToList();
        dbContext.Genres.AddRange(genres);
        await dbContext.SaveChangesAsync();
    }
}
