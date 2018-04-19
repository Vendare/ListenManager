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
        private static VerzeichnisHandler _handler;

        private string _smtpAdress;
        private string _smtpUser;

        private string _accent;
        private string _theme;

        public static ConfigHandler Instance => _instance ?? (_instance = new ConfigHandler());

        private ConfigHandler()
        {
            _handler = VerzeichnisHandler.Instance;

            var configData = _handler.GetConfig();
            SmtpAdress = configData[ConfigType.SmtpAdress];
            SmtpUser = configData[ConfigType.SmtpUser];
            Accent = configData[ConfigType.Accent];
            Theme = configData[ConfigType.Theme];
        }

        public string SmtpAdress
        {
            get => _smtpAdress;
            set
            {
                _smtpAdress = value;
                _handler.SaveConfigParameter(ConfigType.SmtpAdress, _smtpAdress);
            }
        }

        public string SmtpUser
        {
            get => _smtpUser;
            set
            {
                _smtpUser = value;
                _handler.SaveConfigParameter(ConfigType.SmtpUser, _smtpUser);
            }
        }

        public SecureString SmtpPassword
        {
            get
            {
                var raw = _handler.GetSmtpPassword();
                return DecryptData(raw);
            }

            set
            {
                var toSave = EncryptData(value);
                _handler.SaveConfigParameter(ConfigType.SmtpPasswort, toSave);
            }
        }

        public string Accent
        {
            get => _accent;
            set
            {
                if(value == null) return;
                _accent = value;
                _handler.SaveConfigParameter(ConfigType.Accent, _accent);
            }
        }
        public string Theme
        {
            get => _theme;
            set
            {
                if(value == null) return;
                _theme = value;
                _handler.SaveConfigParameter(ConfigType.Theme, _theme);
            }
        }

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