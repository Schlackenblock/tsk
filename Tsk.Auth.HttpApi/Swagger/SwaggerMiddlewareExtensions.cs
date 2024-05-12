using Swashbuckle.AspNetCore.SwaggerUI;

namespace Tsk.Auth.HttpApi.Swagger;

public static class SwaggerMiddlewareExtensions
{
    public static void UseSwagger(this WebApplication webApplication, string environmentName)
    {
        if (webApplication.Environment.IsEnvironment(environmentName))
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI(options => options.CollapseSchemasSection());
        }
    }

    private static void CollapseSchemasSection(this SwaggerUIOptions swaggerUiOptions)
    {
        swaggerUiOptions.DefaultModelsExpandDepth(0);
    }
}
