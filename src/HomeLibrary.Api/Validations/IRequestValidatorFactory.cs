namespace HomeLibrary.Api.Validations;

public interface IRequestValidatorFactory
{
    IRequestValidator? Create(Type requestType);
}