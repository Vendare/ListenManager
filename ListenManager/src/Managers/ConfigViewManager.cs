using System.Collections.Generic;
using System.Security;
using System.Windows;
using MahApps.Metro;

namespace ListenManager.Managers
{
    public class ConfigViewManager : BaseManager
    {
        private string _selectedAccent;
        private string _selectedTheme;

        private SecureString _passwort;
        private string _user;
        private string _smtpServer;

        public List<string> AccentList { get; } = new List<string>()
        {
            "Red", "Green", "Blue", "Purple",
            "Orange", "Lime", "Emerald", "Teal",
            "Cyan", "Cobalt", "Indigo", "Violet",
            "Pink", "Magenta", "Crimson", "Amber",
            "Yellow", "Brown", "Olive", "Steel",
            "Mauve", "Taupe", "Sienna"
        };

        public List<string> ThemeList { get; } = new List<string>()
        {
            "BaseLight", "BaseDark"
        };

        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                OnPropertyChanged(nameof(SelectedAccent));
                SwitchStyle();
            }
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));
                SwitchStyle();
            }
        }

        public string SmtpAdress
        {
            get => _smtpServer;
            set
            {
                _smtpServer = value;
                OnPropertyChanged(nameof(SmtpAdress));
            }
        }

        public string Username
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public SecureString Password
        {
            get => _passwort;
            set
            {
                _passwort = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private void SwitchStyle()
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent),
                ThemeManager.GetAppTheme(SelectedTheme));
        }
    }
}