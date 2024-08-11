using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Features.ForAdmins.Auth;

[ApiController]
[Route("/management/auth")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost]
    [ProducesResponseType<TokenPairDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody][Required] UserCredentialsDto userCredentials)
    {
        var keycloakBaseUrl = configuration["Keycloak:BaseUrl"];
        var keycloakHttpClient = new HttpClient
        {
            BaseAddress = new Uri(keycloakBaseUrl!)
        };

        var form = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", "public-client" },
            { "username", userCredentials.Username },
            { "password", userCredentials.Password }
        };
        var response = await keycloakHttpClient.PostAsync("realms/tsk/protocol/openid-connect/token", new FormUrlEncodedContent(form));

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            var failureResponse = await response.Content.ReadFromJsonAsync<KeycloakFailureResponse>();
            return BadRequest(failureResponse!.ErrorDescription);
        }
        if (response.StatusCode is not HttpStatusCode.OK)
        {
            var failureResponse = await response.Content.ReadFromJsonAsync<KeycloakFailureResponse>();
            throw new Exception($"Keycloak responded with \"{response.StatusCode}\": \"{failureResponse!.ErrorDescription}\" for the \"{userCredentials.Username}\" user.");
        }

        var successResponse = await response.Content.ReadFromJsonAsync<KeycloakSuccessResponse>();
        var tokenPairDto = new TokenPairDto
        {
            AccessToken = new TokenDto
            {
                Value = successResponse!.AccessToken,
                ExpiresIn = successResponse.ExpiresIn
            },
            RefreshToken = new TokenDto
            {
                Value = successResponse.RefreshToken,
                ExpiresIn = successResponse.RefreshExpiresIn
            }
        };
        return Ok(tokenPairDto);
    }

    private class KeycloakSuccessResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }

        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; init; }

        [JsonPropertyName("refresh_expires_in")]
        public required int RefreshExpiresIn { get; init; }
    }

    private class KeycloakFailureResponse
    {
        [JsonPropertyName("error_description")]
        public required string ErrorDescription { get; init; }
    }
}
