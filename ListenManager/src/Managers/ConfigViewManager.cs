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

        private SecureString passwort;
        private SecureString user;
        private string smtpServer;

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

        private void SwitchStyle()
        {
            ThemeManager.ChangeAppStyle(Application.Current, 
                ThemeManager.GetAccent(SelectedAccent),
                ThemeManager.GetAppTheme(SelectedTheme));
        }

        public void loadCredentials()
        {
            
        }
    }
}