using Microsoft.AspNetCore.Http;

namespace HomeLibrary.Api.Requests;

public sealed class ImportBooksRequest
{
    public IFormFile? File { get; set; }
}