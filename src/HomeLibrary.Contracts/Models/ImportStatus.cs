namespace HomeLibrary.Contracts.Models;

public enum ImportStatus
{
    Pending = 0,
    Processing = 1,
    Imported = 2,
    ImportedWithFailures = 3,
    Failed = 4
}