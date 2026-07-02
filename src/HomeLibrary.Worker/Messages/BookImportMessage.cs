namespace HomeLibrary.Worker.Messages;

public record BookImportMessage(
    string Name,
    string Author,
    string Genre
);