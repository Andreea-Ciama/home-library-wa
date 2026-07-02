namespace HomeLibrary.Contracts.Messages;

public record BookImportMessage(
    string Name,
    string Author,
    string Genre
);