namespace HomeLibrary.Contracts.Models;

public class ImportHistory
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = "";

    public string FileHash { get; set; } = "";

    public DateTime ImportDate { get; set; }

    public ImportStatus Status { get; set; }

    public int TotalBooks { get; set; }

    public int ProcessedBooks { get; set; }

    public int ImportedBooks { get; set; }

    public int FailedBooks { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}