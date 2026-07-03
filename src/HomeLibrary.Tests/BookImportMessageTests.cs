using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Tests;

public class BookImportMessageTests
{
    [Fact]
    public void BookImportMessage_Should_Store_Csv_Row_Data()
    {
        var message = new BookImportMessage(
            "Clean Code",
            "Robert C. Martin",
            "Programming"
        );

        Assert.Equal("Clean Code", message.Name);
        Assert.Equal("Robert C. Martin", message.Author);
        Assert.Equal("Programming", message.Genre);
    }
}