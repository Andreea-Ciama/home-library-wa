namespace HomeLibrary.Contracts.Models;

public class Book
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Author { get; set; } = "";

    public string Genre { get; set; } = "";

    public DateTime ImportDate { get; set; }

    public Guid ImportId { get; set; }

    public ImportHistory Import { get; set; } = null!;
}