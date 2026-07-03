namespace HomeLibrary.Contracts.Models;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public DateTime ImportDate { get; set; }
}