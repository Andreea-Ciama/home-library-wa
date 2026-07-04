namespace HomeLibrary.Contracts.Dtos;

public record BookDto(
    Guid Id,
    string Name,
    string Author,
    string Genre,
    DateTime ImportDate
);