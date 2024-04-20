using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.Tests;

internal static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetAsync(
        this HttpClient httpClient,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        object queryParameters)
    {
        var queryParametersString = queryParameters.AsQueryParametersString();
        requestUri = !string.IsNullOrEmpty(queryParametersString)
            ? $"{requestUri}?{queryParametersString}"
            : requestUri;

        return await httpClient.GetAsync(requestUri);
    }

    private static string AsQueryParametersString(this object @object)
    {
        var queryParameters = new List<string>();

        foreach (var property in @object.GetType().GetProperties())
        {
            var isQueryParameter = Attribute.IsDefined(property, typeof(FromQueryAttribute));
            if (isQueryParameter)
            {
                var propertyValue = property.GetValue(@object);
                if (propertyValue is not null)
                {
                    var queryParameter = $"{property.Name}={propertyValue}";
                    queryParameters.Add(queryParameter);
                }
            }
        }

        return string.Join('&', queryParameters);
    }
}
