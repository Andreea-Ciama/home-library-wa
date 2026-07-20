namespace HomeLibrary.Api.Validations;

public interface IRequestValidator
{
    Type RequestType { get; }

    Task<IReadOnlyCollection<string>> ValidateAsync(
        object request,
        CancellationToken cancellationToken);
}

public interface IRequestValidator<in TRequest> : IRequestValidator
{
    Task<IReadOnlyCollection<string>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken);
}
