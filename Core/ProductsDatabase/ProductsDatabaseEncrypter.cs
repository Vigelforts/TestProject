using System;
using System.Collections.Generic;
using System.Text;

namespace Paragon.Container.Core
{
    internal static class ProductsDatabaseEncrypter
    {
        public static string Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);

            List<byte> encodedData = new List<byte>(data.Length);
            foreach (byte b in data)
            {
                byte encodedByte = (byte)(b ^ 42);
                encodedData.Add(encodedByte);
            }

            return Convert.ToBase64String(encodedData.ToArray());
        }
        public static string Decrypt(string encodedString)
        {
            byte[] encodedData = Convert.FromBase64String(encodedString);

            List<byte> data = new List<byte>(encodedData.Length);
            foreach (byte b in encodedData)
            {
                byte decodedByte = (byte)(b ^ 42);
                data.Add(decodedByte);
            }

            return Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
        }
    }
}
