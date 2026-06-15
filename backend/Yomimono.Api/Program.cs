using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yomimono.Application;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure;
using Yomimono.Infrastructure.Data;

if (args is ["seed-user", var email, var password])
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var existing = await userManager.FindByEmailAsync(email);
    if (existing is not null)
    {
        Console.WriteLine("Usuário já existe.");
        return;
    }

    var user = new User
    {
        Id = Guid.NewGuid(),
        UserName = email[..email.IndexOf('@')],
        Email = email,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        foreach (var err in result.Errors)
            Console.WriteLine($"Erro: {err.Description}");
        return;
    }

    Console.WriteLine($"Usuário {email} criado com sucesso.");
    return;
}

var builder2 = WebApplication.CreateBuilder(args);

builder2.Services.AddControllers();
builder2.Services.AddOpenApi();
builder2.Services.AddApplication();
builder2.Services.AddInfrastructure(builder2.Configuration);
builder2.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app2 = builder2.Build();

using (var scope2 = app2.Services.CreateScope())
{
    var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
    await db2.Database.MigrateAsync();
    await SeedData.SeedGenresAsync(db2);
}

if (app2.Environment.IsDevelopment())
{
    app2.MapOpenApi();
}

app2.UseCors("AllowAngular");
app2.UseAuthentication();
app2.UseAuthorization();
app2.MapControllers();

await app2.RunAsync();

public partial class Program { }
