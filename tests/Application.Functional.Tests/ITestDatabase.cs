namespace Application.Functional.Tests;

public interface ITestDatabase
{
    Task InitialiseAsync();
    Task DisposeAsync();
}