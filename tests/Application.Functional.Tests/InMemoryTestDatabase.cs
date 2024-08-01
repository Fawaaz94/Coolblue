using Insurance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Functional.Tests;

public class InMemoryTestDatabase : ITestDatabase
{
    public async Task InitialiseAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options);

        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.EnsureDeletedAsync();
    }
}