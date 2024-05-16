namespace Tsk.Auth.HttpApi.Passwords;

public sealed class BCryptPasswordHandler : IPasswordHandler
{
    public string HashPassword(string plainTextPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(plainTextPassword);
    }

    public bool VerifyPassword(string plainTextPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(plainTextPassword, hashedPassword);
    }
}

public static class BCryptPasswordHandlerDependencyInjection
{
    public static void AddBCryptPasswordHandler(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSingleton<IPasswordHandler, BCryptPasswordHandler>();
    }
}
