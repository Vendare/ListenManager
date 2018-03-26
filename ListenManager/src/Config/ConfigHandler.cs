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

            DecryptData(EncryptData(sec));

            var configData = VerzeichnisHandler.Instance.GetConfig();
            SmtpAdress = configData[ConfigType.SmtpAdress];
            SmtpUser = DecryptData(configData[ConfigType.SmtpUser]);
            SmtpPassword = DecryptData(configData[ConfigType.SmtpPasswort]);
            Accent = configData[ConfigType.Accent];
            Theme = configData[ConfigType.Theme];
        }

        public string SmtpAdress { get; set; }
        public SecureString SmtpUser { get; set; }
        public SecureString SmtpPassword { get; set; }
        public string Accent { get; set; }
        public string Theme { get; set; }

        public string EncryptData(SecureString secStr)
        {
            var size = secStr.Length * 2;
            var raw = Marshal.SecureStringToGlobalAllocUnicode(secStr);

            var array = new byte[size];
            Marshal.Copy(raw, array, 0, size);

            var ciphertext = ProtectedData.Protect(array, null, DataProtectionScope.CurrentUser);

            Marshal.ZeroFreeGlobalAllocUnicode(raw);

            var rueck = Encoding.Unicode.GetString(ciphertext);

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }

            return rueck;
        }

        public SecureString DecryptData(string cypherText)
        {
            var secStr = new SecureString();

            var raw = Encoding.Unicode.GetBytes(cypherText);

            var unpro = ProtectedData.Unprotect(raw, null, DataProtectionScope.CurrentUser);

            for (var i = 0; i < unpro.Length; i++)
            {
                unpro[i] = 0;
            }

            var data = Encoding.Default.GetChars(unpro);

            foreach (var character in data)
            {
                secStr.AppendChar(character);
            }

            return secStr;
        }
    }
}