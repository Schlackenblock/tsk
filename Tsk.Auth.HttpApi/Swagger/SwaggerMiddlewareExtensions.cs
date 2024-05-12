namespace Tsk.Auth.HttpApi.Swagger;

public static class SwaggerMiddlewareExtensions
{
    public static void UseSwagger(this WebApplication webApplication, string environmentName)
    {
        if (webApplication.Environment.IsEnvironment(environmentName))
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();
        }
    }
}
