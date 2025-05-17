using System;

namespace ChatBot.Services.Security
{
    /// <summary>
    /// Interface for security-related operations
    /// </summary>
    public interface ISecurityService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        string HashPassword(string password, out string salt);
        bool VerifyPassword(string password, string hash, string salt);
        string GenerateAntiForgeryToken();
        bool ValidateAntiForgeryToken(string token);
    }
}