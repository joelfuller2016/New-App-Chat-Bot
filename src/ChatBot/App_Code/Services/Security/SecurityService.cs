using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ChatBot.Services.Security
{
    /// <summary>
    /// Service for security-related operations
    /// </summary>
    public class SecurityService : ISecurityService
    {
        // Use a secure key derivation function for encryption
        private const string EncryptionKey = "YourSecureEncryptionKey123!"; // Change this in production!
        private const string TokenCookieName = "AntiForgeryToken";

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            catch (Exception ex)
            {
                // Log the error
                return plainText;
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                return cipherText;
            }
        }

        public string HashPassword(string password, out string salt)
        {
            // Generate a random salt
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                salt = Convert.ToBase64String(saltBytes);
            }

            // Use PBKDF2 for password hashing
            using (var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000))
            {
                byte[] hash = deriveBytes.GetBytes(20);
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            // Hash the password with the same salt
            using (var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000))
            {
                byte[] newHash = deriveBytes.GetBytes(20);
                string newHashString = Convert.ToBase64String(newHash);
                return newHashString == hash;
            }
        }

        public string GenerateAntiForgeryToken()
        {
            string token = Guid.NewGuid().ToString();
            
            // Store in session and cookie
            HttpContext.Current.Session["AntiForgeryToken"] = token;
            
            HttpCookie cookie = new HttpCookie(TokenCookieName, token)
            {
                HttpOnly = true,
                Secure = HttpContext.Current.Request.IsSecureConnection
            };
            
            HttpContext.Current.Response.Cookies.Add(cookie);
            
            return token;
        }

        public bool ValidateAntiForgeryToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            // Check session token
            string sessionToken = HttpContext.Current.Session["AntiForgeryToken"] as string;
            if (string.IsNullOrEmpty(sessionToken))
                return false;

            // Check cookie token
            HttpCookie cookie = HttpContext.Current.Request.Cookies[TokenCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return false;

            // Verify both tokens match the provided token
            return token == sessionToken && token == cookie.Value;
        }
    }
}