using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataFlareClient
{
    public class AesInfo
    {
        public static AesInfo FromPassword(string password)
        {
            var passwordBytes = UnicodeEncoding.Unicode.GetBytes(password);
            var aesKey = SHA256.Create().ComputeHash(passwordBytes);
            var aesIV = MD5.Create().ComputeHash(passwordBytes);
            return new AesInfo(aesKey, aesIV);
        }

        public AesInfo(string key, string iv)
        {
            Key = key;
            IV = iv;
        }

        public AesInfo(byte[] key, byte[] iv)
        {
            Key = Convert.ToHexString(key);
            IV = Convert.ToHexString(iv);
        }

        public string Key { get; set; }
        public string IV { get; set; }
    }
}
