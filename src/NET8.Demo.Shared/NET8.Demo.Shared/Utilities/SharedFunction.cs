using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace NET8.Demo.Shared;

public static class SharedFunction
{
    public static string GenerateSecret(int length = 32)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static async ValueTask<List<T>> LoadJsonAsync<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var json = await reader.ReadToEndAsync();
        return JsonConvert.DeserializeObject<List<T>>(json);
    }

    public static string GenerateUniqueString(int length = 10)
    {
        string randomStr = "abcdefghijklmnopqxyztrABCDEFGHIJKLMNOPQXYZTR1203456789";
        var rndDigits = new StringBuilder().Insert(0, randomStr, length).ToString().ToCharArray();
        return string.Join("", rndDigits.OrderBy(o => Guid.NewGuid()).Take(length));
    }

    public static string GeneratePassword()
    {
        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*";

        var random = new Random();
        var result = new StringBuilder()
            .Append(upperCaseChars[random.Next(upperCaseChars.Length)])
            .Append(lowerCaseChars[random.Next(lowerCaseChars.Length)])
            .Append(digits[random.Next(digits.Length)])
            .Append(specialChars[random.Next(specialChars.Length)]);

        var allChars = upperCaseChars + lowerCaseChars + digits + specialChars;
        for (int i = 4; i < 10; i++)
        {
            result.Append(allChars[random.Next(allChars.Length)]);
        }

        return new string(result.ToString().OrderBy(_ => random.Next()).ToArray());
    }
}
