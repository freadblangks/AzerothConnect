using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace AzerothConnect.Crypto;

public class SRP6
{
    public static byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(32);
    }

    public static byte[] GenerateVerifier(byte[] salt, string username, string password)
    {
        // Convert username and password to uppercase
        string uppercaseUsername = username.ToUpper();
        string uppercasePassword = password.ToUpper();

        // Calculate h1 = SHA1("USERNAME:PASSWORD")
        byte[] h1;
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] usernameAndPasswordBytes = System.Text.Encoding.UTF8.GetBytes($"{uppercaseUsername}:{uppercasePassword}");
            h1 = sha1.ComputeHash(usernameAndPasswordBytes);
        }

        // Calculate h2 = SHA1(salt || h1)
        byte[] h2;
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] saltAndH1 = new byte[salt.Length + h1.Length];
            Buffer.BlockCopy(salt, 0, saltAndH1, 0, salt.Length);
            Buffer.BlockCopy(h1, 0, saltAndH1, salt.Length, h1.Length);
            h2 = sha1.ComputeHash(saltAndH1);
        }

        // Convert h2 to an integer in little-endian order
        BigInteger h2Integer = new BigInteger(h2, true, false);

        // Calculate (g ^ h2) % N
        BigInteger g = 7;
        BigInteger N = BigInteger.Parse("0894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", NumberStyles.HexNumber);
        BigInteger result = BigInteger.ModPow(g, h2Integer, N);

        // Convert the result back to a byte array in little-endian order
        byte[] verifier = result.ToByteArray();

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(verifier);
        }

        return verifier;
    }
}
