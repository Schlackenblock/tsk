using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
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
        var urlEncodedForm = new FormUrlEncodedContent(form);
        var response = await keycloakHttpClient.PostAsync("realms/tsk/protocol/openid-connect/token", urlEncodedForm);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            var failureResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            return BadRequest(failureResponse!["error_description"].GetString());
        }
        if (response.StatusCode is not HttpStatusCode.OK)
        {
            var failureResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            var errorDescription = failureResponse!["error_description"].GetString();

            throw new Exception(
                $"Keycloak responded with \"{response.StatusCode}\": \"{errorDescription}\" " +
                $"for the \"{userCredentials.Username}\" user."
            );
        }

        var successResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        var tokenPairDto = new TokenPairDto
        {
            AccessToken = new TokenDto
            {
                Value = successResponse!["access_token"].GetString()!,
                ExpiresIn = successResponse["expires_in"].GetInt32()
            },
            RefreshToken = new TokenDto
            {
                Value = successResponse["refresh_token"].GetString()!,
                ExpiresIn = successResponse["refresh_expires_in"].GetInt32()
            }
        };
        return Ok(tokenPairDto);
    }
}
