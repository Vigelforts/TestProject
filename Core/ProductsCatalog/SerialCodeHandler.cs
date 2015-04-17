using Cirrious.CrossCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Paragon.Container.Core
{
    internal static class SerialCodeHandler
    {
        public static async Task<SerialCodeResponse> HandleCode(string rawKey)
        {
            SerialCodeResponse response = new SerialCodeResponse();
            response.IsValid = false;
            response.BaseId = string.Empty;

            if (string.IsNullOrEmpty(rawKey))
            {
                return response;
            }

            string key = FormatKey(rawKey);
            if (string.IsNullOrEmpty(key))
            {
                return response;
            }

            if (!ValidatePlatform(key))
            {
                return response;
            }

            ValidateKeyResult validateResult = await ValidateCode(key);
            if (!validateResult.IsValid)
            {
                return response;
            }

            response.IsValid = true;
            response.IsTemporary = validateResult.IsTemporary;
            response.ValidUntil = validateResult.ValidUntil;
            response.BaseId = GetBaseId(key);

            return response;
        }

        private static string GetBaseId(string key)
        {
            return key.Substring(2, 4);
        }

        private static string FormatKey(string key)
        {
            // serial key exampl
            // W8XXXX-XXXXX-XXXXX-XXXXX

            if (key.Length == 24)
            {
                return key.ToUpper();
            }

            if (key.Length == 21)
            {
                string formattedKey = key.ToUpper();
                formattedKey = formattedKey.Insert(6, "-");
                formattedKey = formattedKey.Insert(12, "-");
                formattedKey = formattedKey.Insert(18, "-");

                return formattedKey;
            }

            return string.Empty;
        }
        private static bool ValidatePlatform(string key)
        {
            if (key.StartsWith("W8") || key.StartsWith("DP"))
            {
                return true;
            }

            return false;
        }
        private static async Task<ValidateKeyResult> ValidateCode(string key)
        {
            Common.IDeviceInformation deviceInformation = Mvx.Resolve<Common.IDeviceInformation>();
            string deviceId = deviceInformation.GetDeviceId();

            string request = CreateHttpRequest(deviceId, key);

            Common.IHttpClient httpClient = Mvx.Resolve<Common.IHttpClient>();
            string response = await httpClient.Request(request);
            return GetValidateResult(response);
        }

        private static string CreateHttpRequest(string deviceId, string serialKey)
        {
            Encryption.SerialKeyEncryptor encryptor = new Encryption.SerialKeyEncryptor();

            string uri = "http://sld1.penreader.com/fcgid/sld1.fcg";
            string protocol = "5";
            string encryptedDeviceId = encryptor.Encrypt(deviceId);
            string encryptedSerialKey = encryptor.Encrypt(serialKey);
            string deviceModel = "windows_device";
            string platform = "win8";

            return string.Format("{0}?protocol={1}&p1={2}&p2={3}&p3={4}&platform={5}",
                uri, protocol, encryptedDeviceId, deviceModel, encryptedSerialKey, platform);
        }
        private static ValidateKeyResult GetValidateResult(string serverResponce)
        {
            try
            {
                ValidateKeyResult result = new ValidateKeyResult();
                result.IsValid = false;

                XDocument xmlResponce = XDocument.Parse(serverResponce);
                
                XElement rootElement = xmlResponce.Root;
                if (rootElement.Name != "response")
                {
                    return result;
                }

                XElement serialElement = rootElement.Element("serial");
                if (serialElement.Attribute("status").Value == "accepted")
                {
                    result.IsValid = true;
                }
                else
                {
                    return result;
                }

                XAttribute validUntilAttribute = serialElement.Attribute("valid_until");
                if (validUntilAttribute != null)
                {
                    result.IsTemporary = true;
                    result.ValidUntil = DateTime.Parse(validUntilAttribute.Value);
                }
                else
                {
                    result.IsTemporary = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
                ValidateKeyResult invalidResult = new ValidateKeyResult();
                invalidResult.IsValid = false;
                return invalidResult;
            }
        }
    }

    internal struct SerialCodeResponse
    {
        public bool IsValid { get; set; }
        public bool IsTemporary { get; set; }
        public DateTime ValidUntil { get; set; }
        public string BaseId { get; set; }
    }

    internal struct ValidateKeyResult
    {
        public bool IsValid { get; set; }
        public bool IsTemporary { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
