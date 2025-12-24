using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace TapTap.TapDB.Standalone.Internal {
    public static class Codec {
        private static readonly byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string cachedMacAddress = null;

        public static string Encode(string str) {
            if (str == null) {
                return null;
            }

            var rgbKey = Encoding.UTF8.GetBytes(GetMacAddress().Substring(0, 8));
            var rgbIv = Keys;
            var inputByteArray = Encoding.UTF8.GetBytes(str);
            var dCsp = new DESCryptoServiceProvider();
            var mStream = new MemoryStream();
            var cStream =
                new CryptoStream(mStream, dCsp.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Convert.ToBase64String(mStream.ToArray());
        }

        public static string Decode(string str) {
            if (str == null) {
                return null;
            }

            var rgbKey = Encoding.UTF8.GetBytes(GetMacAddress().Substring(0, 8));
            var rgbIv = Keys;
            var inputByteArray = Convert.FromBase64String(str);
            var cryptoServiceProvider = new DESCryptoServiceProvider();
            var mStream = new MemoryStream();
            var cStream =
                new CryptoStream(mStream, cryptoServiceProvider.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        private static string GetMacAddress() {
            if (!string.IsNullOrEmpty(cachedMacAddress)) {
                return cachedMacAddress;
            }

            var physicalAddress = "FFFFFFFFFFFF";
            try {
                var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var networkInterface in allNetworkInterfaces) {
                    var address = networkInterface.GetPhysicalAddress().ToString();
                    if (string.IsNullOrEmpty(address)) continue;
                    physicalAddress = address;
                    break;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            cachedMacAddress = physicalAddress;
            return cachedMacAddress;
        }
    }
}