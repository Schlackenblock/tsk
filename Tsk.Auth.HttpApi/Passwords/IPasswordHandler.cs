namespace Tsk.Auth.HttpApi.Passwords;

public interface IPasswordHandler
{
    string HashPassword(string plainTextPassword);

    bool VerifyPassword(string plainTextPassword, string hashedPassword);
}
