using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;          

namespace RoomReservation.Api.Extensions
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info = new()
                    {
                        Title = "Room Reservation API",
                        Version = "v1"
                    };
                    return Task.CompletedTask;
                });

                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            return services;
        }
    }

    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider
    ) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken ct)
        {
            var authSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!authSchemes.Any(s => s.Name == "Bearer"))
                return;

            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Podaj JWT token"
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add("Bearer", bearerScheme);

            // Nowy sposób referencji w OpenApi 2.x — przez OpenApiSecuritySchemeReference
            foreach (var path in document.Paths.Values)
            {
                foreach (var operation in path.Operations.Values)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                }
            }
        }
    }
}