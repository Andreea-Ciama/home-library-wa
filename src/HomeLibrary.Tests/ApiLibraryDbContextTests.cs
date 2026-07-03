using HomeLibrary.Api.Data;
using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Tests;

public class ApiLibraryDbContextTests
{
    [Fact]
    public async Task Can_Add_And_Read_Book()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new LibraryDbContext(options);

        db.Books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Name = "The Hobbit",
            Author = "J.R.R. Tolkien",
            Genre = "Fantasy",
            ImportDate = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var books = await db.Books.ToListAsync();

        Assert.Single(books);
        Assert.Equal("The Hobbit", books[0].Name);
    }
}