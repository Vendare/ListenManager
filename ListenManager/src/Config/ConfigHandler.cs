using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using ListenManager.Database.Handlers;
using ListenManager.Enums;

namespace ListenManager.Config
{
    public class ConfigHandler
    {
        private static ConfigHandler _instance;

        public static ConfigHandler Instance => _instance ?? (_instance = new ConfigHandler());

        private ConfigHandler()
        {
            var sec = new SecureString();
            sec.AppendChar('b');
            sec.AppendChar('l');
            sec.AppendChar('a');

            var encrypt = EncryptData(sec);
            var decrypt = DecryptData(encrypt);

            var configData = VerzeichnisHandler.Instance.GetConfig();
            SmtpAdress = configData[ConfigType.SmtpAdress];
            SmtpUser = configData[ConfigType.SmtpUser];
            SmtpPassword = DecryptData(configData[ConfigType.SmtpPasswort]);
            Accent = configData[ConfigType.Accent];
            Theme = configData[ConfigType.Theme];
        }

        public string SmtpAdress { get; set; }
        public string SmtpUser { get; set; }
        public SecureString SmtpPassword { get; set; }
        public string Accent { get; set; }
        public string Theme { get; set; }

        private static string EncryptData(SecureString secStr)
        {
            var rueck = string.Empty;
            if (secStr == null) return rueck;

            var size = secStr.Length * 2;
            var raw = Marshal.SecureStringToGlobalAllocUnicode(secStr);

            var array = new byte[size];
            Marshal.Copy(raw, array, 0, size);

            Marshal.ZeroFreeGlobalAllocUnicode(raw);

            var ciphertext = ProtectedData.Protect(array, null, DataProtectionScope.CurrentUser);

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }

            rueck = Convert.ToBase64String(ciphertext);
            
            return rueck;
        }

        private static SecureString DecryptData(string cypherText)
        {
            if (cypherText == null || cypherText.Equals(string.Empty)) return null;
            var secStr = new SecureString();

            var raw = Convert.FromBase64String(cypherText);

            var unpro = ProtectedData.Unprotect(raw, null, DataProtectionScope.CurrentUser);

            var data = Encoding.Unicode.GetChars(unpro);

            for (var i = 0; i < unpro.Length; i++)
            {
                unpro[i] = 0;
            }

            for (var i = 0; i < data.Length; i++)
            {
                secStr.AppendChar(data[i]);
                data[i] = '\0';
            }

            return secStr;
        }
    }
}