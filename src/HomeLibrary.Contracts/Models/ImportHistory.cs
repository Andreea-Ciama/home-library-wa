namespace HomeLibrary.Contracts.Models;

public class ImportHistory
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = "";

    public string FileHash { get; set; } = "";

    public DateTime ImportDate { get; set; }
}