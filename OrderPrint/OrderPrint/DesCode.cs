using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace OrderPrint
{
    class DesCode
    {
        public static string DESEncode(string encryptString, string encryptKey)
        {
            if (encryptKey.Length != 8)
            {
                return null;
            }
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            //             byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
            //             byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            byte[] rgbKey = Encoding.GetEncoding("GBK").GetBytes(encryptKey);
            byte[] inputByteArray = Encoding.GetEncoding("GBK").GetBytes(encryptString);

            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, new byte[8]), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        public static string DESDecode(string decryptString, string decryptKey)
        {
            if (decryptKey.Length != 8)
            {
                return null;
            }
            /*
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            */
            byte[] rgbKey = Encoding.GetEncoding("GBK").GetBytes(decryptKey);
            byte[] inputByteArray = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, new byte[8]), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.GetEncoding("GBK").GetString(mStream.ToArray());
        }
    }
}
