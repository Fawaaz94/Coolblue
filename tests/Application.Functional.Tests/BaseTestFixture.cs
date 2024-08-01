using Insurance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Functional.Tests;

[TestFixture]
public abstract class BaseTestFixture
{
    protected ApplicationDbContext _context;

    [SetUp]
    public async Task TestSetUp()
    {   
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);

        await _context.Database.EnsureCreatedAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}
