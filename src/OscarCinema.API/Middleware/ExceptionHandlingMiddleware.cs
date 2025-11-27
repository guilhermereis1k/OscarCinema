using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Domain.Validation;

namespace OscarCinema.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AutoMapperMappingException ex) when (ex.InnerException is DomainExceptionValidation domainEx)
            {
                _logger.LogWarning(domainEx, "Domain validation failed during mapping: {Message}", domainEx.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var problem = new ProblemDetails
                {
                    Title = "Invalid data",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = domainEx.Message
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            catch (DomainExceptionValidation ex)
            {
                _logger.LogWarning(ex, "Domain validation failed: {Message}", ex.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var problem = new ProblemDetails
                {
                    Title = "Invalid data",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails
                {
                    Title = "Server error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}