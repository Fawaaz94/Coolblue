namespace Application.Functional.Tests;

public static class TestDatabaseFactory
{
    // Here you can add multiple databases
    public static async Task<ITestDatabase> CreateAsync()
    {
        var database = new InMemoryTestDatabase();
        await database.InitialiseAsync();
        return database;
    }
}