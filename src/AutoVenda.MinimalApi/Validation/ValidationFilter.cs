using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;

namespace AutoVenda.MinimalApi.Validation;

/// <summary>
/// Endpoint filter que valida o primeiro argumento do tipo T (request body) com DataAnnotations e retorna 400 + ProblemDetails em caso de erro.
/// </summary>
public static class ValidationFilter
{
    public static RouteHandlerBuilder ValidateRequest<TRequest>(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var request = context.Arguments
                .OfType<TRequest>()
                .FirstOrDefault();

            if (request is null)
                return await next(context);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                results,
                validateAllProperties: true);

            if (isValid)
                return await next(context);

            var errors = results
                .SelectMany(r => r.MemberNames.Select(m => new KeyValuePair<string, string[]>(m, new[] { r.ErrorMessage ?? "Invalid" })))
                .GroupBy(x => x.Key)
                .ToDictionary(g => g.Key, g => g.SelectMany(x => x.Value).Distinct().ToArray());

            return Results.ValidationProblem(new Dictionary<string, string[]>(errors));
        });
        return builder;
    }
}

