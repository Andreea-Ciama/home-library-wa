using HomeLibrary.Contracts.Models;

namespace HomeLibrary.Tests;

public class BookTests
{
    [Fact]
    public void Book_Should_Have_Guid_Id()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Name = "Dune",
            Author = "Frank Herbert",
            Genre = "Sci-Fi",
            ImportDate = DateTime.UtcNow
        };

        Assert.NotEqual(Guid.Empty, book.Id);
        Assert.Equal("Dune", book.Name);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal("Sci-Fi", book.Genre);
    }
}