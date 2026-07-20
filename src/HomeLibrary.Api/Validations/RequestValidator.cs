namespace HomeLibrary.Api.Validations;

public abstract class RequestValidator<TRequest> : IRequestValidator<TRequest>
{
    public Type RequestType => typeof(TRequest);

    public abstract Task<IReadOnlyCollection<string>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<string>> ValidateAsync(
        object request,
        CancellationToken cancellationToken)
    {
        if (request is not TRequest typedRequest)
        {
            throw new ArgumentException(
                $"Validatorul {typeof(TRequest).Name} nu poate valida {request.GetType().Name}.");
        }

        return ValidateAsync(typedRequest, cancellationToken);
    }
}