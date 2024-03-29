﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DataFlareClient
{
    public static class EncryptedFlare
    {
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=net-5.0

        public static Flare Create(string data, AesInfo info)
        {
            return Create(data, info.Key, info.IV);
        }

        static Flare Create(string data, string key, string iv)
        {
            return Create(data, ConvertBackports.FromHexString(key), ConvertBackports.FromHexString(iv));
        }

        static Flare Create(string data, byte[] key, byte[] iv)
        {
            var encryptedData = EncryptStringToBytes_Aes(data, key, iv);
            var encryptedString = Convert.ToBase64String(encryptedData);

            var flare = new Flare(encryptedString);
            flare.Sign(key, iv);
            return flare;
        }

        public static string Decrypt(this Flare flare, AesInfo info)
        {
            return flare.Decrypt(info.Key, info.IV);
        }

        static string Decrypt(this Flare flare, string key, string iv)
        {
            return flare.Decrypt(ConvertBackports.FromHexString(key), ConvertBackports.FromHexString(iv));
        }

        static string Decrypt(this Flare flare, byte[] key, byte[] iv)
        {
            try
            {
                var decString = DecryptStringFromBytes_Aes(Convert.FromBase64String(flare.Data), key, iv);
                return decString;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return default;
            }
        }

        public static (T data, bool encrypted) TryDecrypt<T>(this Flare flare, AesInfo info)
        {
            try
            {
                if (info == null)
                {
                    return ((T)flare.Value(typeof(T)), false);
                }
            }
            catch(Exception ex)
            {
                return default;
            }

            var (json, encrypted) = flare.TryDecrypt(info);
            try
            {
                return (JsonSerializer.Deserialize<T>(json), encrypted);
            }
            catch(Exception ex)
            {
                return (default, encrypted);
            }
        }

        public static (string data, bool encrypted) TryDecrypt(this Flare flare, AesInfo info)
        {
            var result = flare.Decrypt(info);
            return (result ?? flare.Data, result != null);
        }

        static (string data, bool encrypted) TryDecrypt(this Flare flare, string key, string iv)
        {
            var result = flare.Decrypt(key, iv);
            return (result ?? flare.Data, result != null);
        }

        static (string data, bool encrypted) TryDecrypt(this Flare flare, byte[] key, byte[] iv)
        {
            var result = Decrypt(flare, key, iv);
            return (result ?? flare.Data, result != null);
        }


        static string DataMD5(this Flare flare)
        {
            using(var md5 = MD5.Create())
            {
                return ConvertBackports.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(flare.Data)));
            }
        }

        static string EncryptedHash(this Flare flare, AesInfo info)
        {
            return EncryptedHash(flare, info.Key, info.IV);
        }

        static string EncryptedHash(this Flare flare, string key, string iv)
        {
            return EncryptedHash(flare, ConvertBackports.FromHexString(key), ConvertBackports.FromHexString(iv));
        }

        static string EncryptedHash(this Flare flare, byte[] key, byte[] iv)
        {
            var hash = flare.DataMD5();
            return ConvertBackports.ToHexString(EncryptStringToBytes_Aes(hash, key, iv));
        }

        public static void Sign(this Flare flare, AesInfo info)
        {
            flare.Sign(info.Key, info.IV);
        }

        static void Sign(this Flare flare, string key, string iv)
        {
            flare.Sign(ConvertBackports.FromHexString(key), ConvertBackports.FromHexString(iv));
        }

        static void Sign(this Flare flare, byte[] key, byte[] iv)
        {
            var signature = flare.EncryptedHash(key, iv);
            flare.Signature = signature;
        }


        public static bool VerifySignature(this Flare flare, AesInfo info)
        {
            if(info == null)
            {
                if (!string.IsNullOrWhiteSpace(flare.Signature))
                {
                    return false;
                }
                return true;
            }

            return flare.Signature == flare.EncryptedHash(info);
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