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
}
