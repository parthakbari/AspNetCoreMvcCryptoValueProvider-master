using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace AspNetCoreMvcCryptoValueProvider.ValueProvider
{
    public sealed class CryptoNew
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Crypto"/> class from being created.
        /// </summary>
        private CryptoNew()
        {
        }

        /// <summary>
        /// The _key
        /// </summary>
        private static string key = "!hegic@1#%";

        /// <summary>
        /// The delimeter
        /// </summary>
        private static string delimeter = "#@!";

        /// <summary>
        /// Gets the rijndael managed.
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <returns>RijndaelManaged.</returns>
        public static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        /// <summary>
        /// Encrypts the specified key value.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns>System.String.</returns>
        public static string Encrypt(Dictionary<string, string> keyValue)
        {
            string encryptedText = string.Empty;

            foreach (KeyValuePair<string, string> key in keyValue)
            {
                encryptedText += key.Key + "=" + key.Value;
                encryptedText += ";";
            }

            encryptedText = Encrypt(encryptedText);

            return encryptedText.Replace('=', '-').Replace('/', '_').Replace('+', ',');
        }

        /// <summary>
        /// Encrypts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Encrypt(string key, string value)
        {
            string encryptedText = key + "=" + value + ";"; ;
            encryptedText = Encrypt(encryptedText);
            return encryptedText.Replace('=', '-').Replace('/', '_').Replace('+', ',');
        }

        /// <summary>
        /// Creates the secure token.
        /// </summary>
        /// <param name="secretText">The secret text.</param>
        /// <returns>System.String.</returns>
        public static string CreateSecureToken(string secretText)
        {
            //secretText = secretText + delimeter + DateTime.UtcNow.Ticks.ToString();
            //var plainBytes = Encoding.UTF8.GetBytes(secretText);
            //return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(ConfigHelper.GetString("SecureToken", string.Empty))));
            return Encrypt(secretText);
        }

        /// <summary>
        /// Determines whether [is valid secure token] [the specified secure token].
        /// </summary>
        /// <param name="secureToken">The secure token.</param>
        /// <returns><c>true</c> if [is valid secure token] [the specified secure token]; otherwise, <c>false</c>.</returns>
        public static bool IsValidSecureToken(string secureToken)
        {
            bool isValidToken = false;// Default false
            try
            {
                //Decrypt the token
                string decryptedText = Decrypt(secureToken);

                //Split to find utc ticks and code
                string securetoken = "HEGIC";

                if (decryptedText == securetoken)
                {
                    isValidToken = true;
                }
                return isValidToken;
            }
            catch
            {
                return isValidToken;
            }
        }

        /// <summary>
        /// Gets the secret text.
        /// </summary>
        /// <param name="secureToken">The secure token.</param>
        /// <returns>System.String.</returns>
        public static string GetSecretText(string secureToken)
        {
            string secretText = string.Empty;
            try
            {
                var encryptedBytes = Convert.FromBase64String(secureToken);
                //Decrypt the token
                string decryptedText = Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged("HEGIC")));
                //Split to find utc ticks and code
                string[] tokens = decryptedText.Split(new string[] { delimeter }, StringSplitOptions.None);
                if (tokens.Length == 2)
                {
                    secretText = Convert.ToString(tokens[0]);
                }
                return secretText;
            }
            catch
            {
                return secretText;
            }
        }

        /// <summary>
        /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        /// </summary>
        /// <param name="plaintext">Plain text to encrypt</param>
        /// <returns>Base64 encoded string</returns>
        public static String Encrypt(String plaintext)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }

        /// <summary>
        /// Encrypts the specified plain bytes.
        /// </summary>
        /// <param name="plainBytes">The plain bytes.</param>
        /// <param name="rijndaelManaged">The rijndael managed.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        /// <summary>
        /// Decrypts the specified encrypted data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="rijndaelManaged">The rijndael managed.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        /// <summary>
        /// Decrypts the in key value.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> DecryptInKeyValue(string encryptedText)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            encryptedText = encryptedText.Replace('-', '=').Replace('_', '/').Replace(',', '+');
            string decryptedText = Decrypt(encryptedText);

            if (decryptedText != null)
            {
                string[] keyPair = decryptedText.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string key in keyPair)
                {
                    string[] keyValue = key.Split(new char[] { '=' });
                    dictionary.Add(keyValue[0].ToUpper(), keyValue[1]);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        /// </summary>
        /// <param name="encryptedText">Base64 Encoded String</param>
        /// <returns>Decrypted String</returns>
        public static String Decrypt(String encryptedText)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
            }
            catch
            {
                return null;
            }
        }
    }
}




