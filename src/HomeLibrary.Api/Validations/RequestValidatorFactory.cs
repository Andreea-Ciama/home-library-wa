namespace HomeLibrary.Api.Validations;

public sealed class RequestValidatorFactory : IRequestValidatorFactory
{
    private readonly Dictionary<Type, IRequestValidator> _validators;

    public RequestValidatorFactory(IEnumerable<IRequestValidator> validators)
    {
        _validators = validators.ToDictionary(
            validator => validator.RequestType,
            validator => validator);
    }

    public IRequestValidator? Create(Type requestType)
    {
        return _validators.GetValueOrDefault(requestType);
    }
}