using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tsk.Auth.HttpApi.Swagger;

public static class SwaggerGenerationExtensions
{
    public static void UseUniqueSchemaIds(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(
            type =>
            {
                // The "+" sign usually placed between the outer and nested type names (e.g. OuterClass+InnerClass).
                // We replace it with the ":" since it's not unsupported by the SwaggerUI.
                return type.ToString().Replace("+", ":");
            }
        );
    }

    public static void AddJwtAuthentication(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme
        {
            Name = "Bearer",
            BearerFormat = "JWT",
            Scheme = "bearer",
            Description = "Provide JWT Access Token.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "jwt_auth",
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        });
    }
}
