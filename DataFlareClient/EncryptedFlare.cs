using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DataFlareClient
{
    public static class EncryptedFlare
    {
        public static string Decrypt(this Flare flare, AesInfo info)
        {
            return flare.Decrypt(info.Key, info.IV);
        }

        public static string Decrypt(this Flare flare, string key, string iv)
        {
            return flare.Decrypt(Convert.FromHexString(key), Convert.FromHexString(iv));
        }

        public static string Decrypt(this Flare flare, byte[] key, byte[] iv)
        {
            try
            {
                var decString = DecryptStringFromBytes_Aes(Convert.FromHexString(flare.Data), key, iv);
                return decString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        static string DataHash(this Flare flare)
        {
            return flare.Data.GetHashCode().ToString();
        }

        static string EncryptedHash(this Flare flare, AesInfo info)
        {
            var hash = flare.DataHash();
            return Convert.ToHexString(EncryptStringToBytes_Aes(hash, Convert.FromHexString(info.Key), Convert.FromHexString(info.IV)));
        }

        public static void Sign(this Flare flare, AesInfo info)
        {
            var signature = flare.EncryptedHash(info);
            flare.Signature = signature;
        }

        public static bool VerifyEncryptedHash(this Flare flare, AesInfo info)
        {
            return flare.Signature == flare.EncryptedHash(info);
        }

        public static Flare Create(string data, AesInfo info)
        {
            return Create(data, info.Key, info.IV);
        }

        public static Flare Create(string data, string key, string iv)
        {
            return Create(data, Convert.FromHexString(key), Convert.FromHexString(iv));
        }

        public static Flare Create(string data, byte[] key, byte[] iv)
        {
            var encryptedData = EncryptStringToBytes_Aes(data, key, iv);
            var encryptedString = Convert.ToHexString(encryptedData);

            var flare = new Flare(encryptedString);
            return flare;
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}