using BeCause.NuGet.Exceptions;
using HotChocolate.Execution;

namespace HealthTracker.Api.GraphQl.ErrorHandling;

/// <summary>
/// Global error filter for GraphQL that transforms application/domain exceptions
/// into proper GraphQL error responses following the GraphQL specification.
/// </summary>
public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        var exception = error.Exception;

        return exception switch
        {
            NotFoundException notFoundEx => CreateNotFoundError(error, notFoundEx),
            ConflictException conflictEx => CreateConflictError(error, conflictEx),
            FluentValidation.ValidationException fluentValidationEx => CreateFluentValidationError(error, fluentValidationEx),
            ValidationException validationEx => CreateValidationError(error, validationEx),
            UnauthorizedAccessException unauthorizedEx => CreateUnauthorizedError(error, unauthorizedEx),
            ArgumentException argEx => CreateArgumentError(error, argEx),
            InvalidOperationException invalidOpEx => CreateInvalidOperationError(error, invalidOpEx),
            _ => HandleUnknownError(error, exception)
        };
    }

    private IError CreateNotFoundError(IError originalError, NotFoundException exception)
    {
        return ErrorBuilder.FromError(originalError)
            .SetMessage(exception.Message)
            .SetCode("NOT_FOUND")
            .Build();
    }

    private IError CreateConflictError(IError originalError, ConflictException exception)
    {
        return ErrorBuilder.FromError(originalError)
            .SetMessage(exception.Message)
            .SetCode("CONFLICT")
            .Build();
    }

    private IError CreateFluentValidationError(IError originalError, FluentValidation.ValidationException exception)
    {
        return ErrorBuilder.FromError(originalError)
            .SetMessage("One or more validation errors occurred.")
            .SetCode("VALIDATION_ERROR")
            .SetExtension("errors", exception.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage,
                attemptedValue = e.AttemptedValue,
                severity = e.Severity.ToString()
            }).ToList())
            .Build();
    }

    private IError CreateValidationError(IError originalError, ValidationException exception)
    {
        var builder = ErrorBuilder.FromError(originalError)
            .SetMessage(exception.Message ?? "Validation failed")
            .SetCode("VALIDATION_ERROR");

        if (exception.Errors?.Any() == true)
        {
            builder.SetExtension("errors", exception.Errors.Select(e => new
            {
                field = e.Key,
                messages = e.Value
            }).ToList());
        }

        return builder.Build();
    }

    private IError CreateUnauthorizedError(IError originalError, UnauthorizedAccessException exception)
    {
        return ErrorBuilder.FromError(originalError)
            .SetMessage("You are not authorized to perform this operation.")
            .SetCode("UNAUTHORIZED")
            .Build();
    }

    private IError CreateArgumentError(IError originalError, ArgumentException exception)
    {
        var builder = ErrorBuilder.FromError(originalError)
            .SetMessage(exception.Message)
            .SetCode("BAD_REQUEST");

        if (!string.IsNullOrEmpty(exception.ParamName))
        {
            builder.SetExtension("parameterName", exception.ParamName);
        }

        return builder.Build();
    }

    private IError CreateInvalidOperationError(IError originalError, InvalidOperationException exception)
    {
        return ErrorBuilder.FromError(originalError)
            .SetMessage(exception.Message)
            .SetCode("INVALID_OPERATION")
            .Build();
    }

    private IError HandleUnknownError(IError originalError, Exception? exception)
    {
        // In production, return a generic error message
        // The original error with full details will be logged by HotChocolate's diagnostic event listener
        return ErrorBuilder.FromError(originalError)
            .SetMessage("An unexpected error occurred while processing your request.")
            .SetCode("INTERNAL_ERROR")
            .Build();
    }
}
