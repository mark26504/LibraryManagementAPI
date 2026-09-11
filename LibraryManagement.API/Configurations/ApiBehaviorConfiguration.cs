namespace LibraryManagement.API.Configurations
{
    public static class ApiBehaviorConfiguration
    {
        public static IMvcBuilder AddFrozenValidationProblemDetails(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = new Dictionary<string, string[]>();

                    foreach (var entry in context.ModelState)
                    {
                        if (entry.Value is null || entry.Value.Errors.Count == 0)
                            continue;

                        var key = CamelCaseModelStateKey(entry.Key);

                        errors[key] = entry.Value.Errors
                            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                ? "The value is invalid."
                                : e.ErrorMessage)
                            .ToArray();
                    }

                    var problem = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "One or more validation errors occurred.",
                        Instance = context.HttpContext.Request.Path
                    };

                    problem.Extensions["traceId"] =
                        Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                    problem.Extensions["errors"] = errors;

                    return new BadRequestObjectResult(problem);
                };
            });

            return builder;
        }

        private static string CamelCaseModelStateKey(string key)
        {
            if (string.IsNullOrEmpty(key) || key == "$")
                return key;

            var segments = key.Split('.');

            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length > 0)
                    segments[i] = char.ToLowerInvariant(segments[i][0]) + segments[i][1..];
            }

            return string.Join('.', segments);
        }
    }
}