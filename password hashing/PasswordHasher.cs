namespace SrijanDEEP.API.Helpers;

public static class PasswordHasher
{
    public static string Hash(string plainTextPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 11);

    public static bool Verify(string plainTextPassword, string hashedPassword)
        => BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
}