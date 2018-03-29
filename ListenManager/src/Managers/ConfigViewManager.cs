using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ListenManager.Config;
using ListenManager.Database.DataObjects;
using MahApps.Metro;

namespace ListenManager.Managers
{
    public class ConfigViewManager : BaseManager
    {
        private SecureString _passwort;
        private string _user;
        private string _smtpServer;

        private ICommand _updateAccentsCommand;
        private ICommand _updateThemesCommand;

        public ConfigViewManager()
        {
            var config = ConfigHandler.Instance;

            SelectedAccent = (from at in AccentList
                where at.Name.Equals(config.Accent)
                select at).First();
            SelectedAccent.IsChecked = true;

            SelectedTheme = (from tt in ThemeList
                where tt.Name.Equals(config.Theme)
                select tt).First();
            SelectedTheme.IsChecked = true;

            SwitchStyle();

            Username = config.SmtpUser;
            SmtpAdress = config.SmtpAdress;
        }

        public List<AccentToggles> AccentList { get; } = new List<AccentToggles>()
        {
            new AccentToggles() { IsChecked = false, Name = "Red" },
            new AccentToggles() { IsChecked = false, Name = "Green" },
            new AccentToggles() { IsChecked = false, Name = "Blue" },
            new AccentToggles() { IsChecked = false, Name = "Purple" },
            new AccentToggles() { IsChecked = false, Name = "Orange" },
            new AccentToggles() { IsChecked = false, Name = "Lime" },
            new AccentToggles() { IsChecked = false, Name = "Emerald" },
            new AccentToggles() { IsChecked = false, Name = "Teal" },
            new AccentToggles() { IsChecked = false, Name = "Cyan" },
            new AccentToggles() { IsChecked = false, Name = "Cobalt" },
            new AccentToggles() { IsChecked = false, Name = "Indigo" },
            new AccentToggles() { IsChecked = false, Name = "Violet" },
            new AccentToggles() { IsChecked = false, Name = "Pink" },
            new AccentToggles() { IsChecked = false, Name = "Magenta" },
            new AccentToggles() { IsChecked = false, Name = "Crimson" },
            new AccentToggles() { IsChecked = false, Name = "Amber" },
            new AccentToggles() { IsChecked = false, Name = "Yellow" },
            new AccentToggles() { IsChecked = false, Name = "Brown" },
            new AccentToggles() { IsChecked = false, Name = "Olive" },
            new AccentToggles() { IsChecked = false, Name = "Steel" },
            new AccentToggles() { IsChecked = false, Name = "Mauve" },
            new AccentToggles() { IsChecked = false, Name = "Taupe" },
            new AccentToggles() { IsChecked = false, Name = "Sienna"}
        };

        public List<AccentToggles> ThemeList { get; } = new List<AccentToggles>()
        {
            new AccentToggles() {  IsChecked = false, Name = "BaseLight" },
            new AccentToggles() {  IsChecked = false, Name = "BaseDark" }
        };

        public AccentToggles SelectedAccent { get; set; }

        public AccentToggles SelectedTheme { get; set; }

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

        public ICommand UpdateAccentsCommand =>
            _updateAccentsCommand ?? (_updateAccentsCommand = new RelayCommand(UpdateAccents));

        public ICommand UpdateThemmesCommand =>
            _updateThemesCommand ?? (_updateThemesCommand = new RelayCommand(UpdateThemes));

        private void UpdateAccents()
        {
            foreach (var accent in AccentList)
            {
                if (accent.IsChecked && accent.Equals(SelectedAccent))
                {
                    accent.IsChecked = false;
                }

                if (accent.IsChecked && !accent.Equals(SelectedAccent))
                {
                    accent.IsChecked = true;
                    SelectedAccent = accent;
                }
            }
            SwitchStyle();
        }

        private void UpdateThemes()
        {
            foreach (var theme in ThemeList)
            {
                if (theme.IsChecked && theme.Equals(SelectedTheme))
                {
                    theme.IsChecked = false;
                }
                if (theme.IsChecked && !theme.Equals(SelectedAccent))
                {
                    theme.IsChecked = true;
                    SelectedTheme = theme;
                }
            }
            SwitchStyle();
        }

        private void SwitchStyle()
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent.Name),
                ThemeManager.GetAppTheme(SelectedTheme.Name));
        }
    }
}