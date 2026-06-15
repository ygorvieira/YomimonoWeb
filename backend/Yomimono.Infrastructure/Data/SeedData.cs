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

    private static readonly string[] DefaultAuthors =
    [
        "Machado de Assis",
        "Clarice Lispector",
        "Jorge Amado",
        "José Saramago",
        "Guimarães Rosa",
        "Cecília Meireles",
        "Carlos Drummond de Andrade",
        "Fernando Pessoa",
        "Eça de Queirós",
        "Mário de Andrade"
    ];

    public static async Task SeedGenresAsync(AppDbContext dbContext)
    {
        if (await dbContext.Genres.AnyAsync())
            return;

        var genres = DefaultGenres.Select(Genre.Create).ToList();
        dbContext.Genres.AddRange(genres);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedAuthorsAsync(AppDbContext dbContext)
    {
        if (await dbContext.Authors.AnyAsync())
            return;

        foreach (var name in DefaultAuthors)
        {
            var (author, _) = Author.Create(name);
            if (author is not null)
                dbContext.Authors.Add(author);
        }
        await dbContext.SaveChangesAsync();
    }
}
