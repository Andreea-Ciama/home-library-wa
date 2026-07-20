namespace HomeLibrary.Contracts.Messages;

public sealed record BookImportMessage(
    Guid ImportId,
    string Name,
    string Author,
    string Genre);