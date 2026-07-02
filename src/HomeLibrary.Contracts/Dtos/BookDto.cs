namespace HomeLibrary.Contracts.Dtos;

public record BookDto(
    string Name,
    string Author,
    string Genre,
    DateTime ImportDate
);