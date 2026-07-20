using HomeLibrary.Api.Validations;
using HomeLibrary.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeLibrary.Api.Filters;

public sealed class RequestValidationFilter : IAsyncActionFilter
{
    private readonly IRequestValidatorFactory _validatorFactory;

    public RequestValidationFilter(
        IRequestValidatorFactory validatorFactory)
    {
        _validatorFactory = validatorFactory;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var actionArgument in context.ActionArguments.Values)
        {
            if (actionArgument is null)
            {
                continue;
            }

            var validator = _validatorFactory.Create(
                actionArgument.GetType());

            if (validator is null)
            {
                continue;
            }

            var errors = await validator.ValidateAsync(
                actionArgument,
                context.HttpContext.RequestAborted);

            if (errors.Count == 0)
            {
                continue;
            }

            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Fail(errors.ToArray()));

            return;
        }

        await next();
    }
}