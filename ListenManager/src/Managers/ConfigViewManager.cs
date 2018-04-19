using System.Collections.Generic;
using System.Linq;
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
        private readonly ConfigHandler _config;

        private string _user;
        private string _smtpServer;

        private ICommand _updateAccentsCommand;
        private ICommand _updateThemesCommand;
        private ICommand _saveCommand;

        public ConfigViewManager()
        {
            _config = ConfigHandler.Instance;

            SelectedAccent = (from at in AccentList
                where at.Name.Equals(_config.Accent)
                select at).First();
            SelectedAccent.IsChecked = true;

            SelectedTheme = (from tt in ThemeList
                where tt.Name.Equals(_config.Theme)
                select tt).First();
            SelectedTheme.IsChecked = true;

            SwitchStyle();

            Username = _config.SmtpUser;
            SmtpAdress = _config.SmtpAdress;
        }

        public List<AccentToggles> AccentList { get; } = new List<AccentToggles>()
        {
            new AccentToggles() { IsChecked = false, Name = "Red", DisplayName= "Rot" },
            new AccentToggles() { IsChecked = false, Name = "Green", DisplayName = "Green" },
            new AccentToggles() { IsChecked = false, Name = "Blue", DisplayName = "Blau" },
            new AccentToggles() { IsChecked = false, Name = "Purple", DisplayName = "Purpur" },
            new AccentToggles() { IsChecked = false, Name = "Orange", DisplayName = "Orange" },
            new AccentToggles() { IsChecked = false, Name = "Lime", DisplayName = "Lindgrün" },
            new AccentToggles() { IsChecked = false, Name = "Emerald", DisplayName = "Smaragdgrün" },
            new AccentToggles() { IsChecked = false, Name = "Teal", DisplayName = "Türkis" },
            new AccentToggles() { IsChecked = false, Name = "Cyan", DisplayName = "Cyanblau" },
            new AccentToggles() { IsChecked = false, Name = "Cobalt", DisplayName = "Kobaltblau" },
            new AccentToggles() { IsChecked = false, Name = "Indigo", DisplayName = "Indigoblau" },
            new AccentToggles() { IsChecked = false, Name = "Violet", DisplayName = "Violett" },
            new AccentToggles() { IsChecked = false, Name = "Pink", DisplayName = "Rosa" },
            new AccentToggles() { IsChecked = false, Name = "Magenta", DisplayName = "Magenta" },
            new AccentToggles() { IsChecked = false, Name = "Crimson", DisplayName = "Karminrot" },
            new AccentToggles() { IsChecked = false, Name = "Amber", DisplayName = "Bernsteingelb" },
            new AccentToggles() { IsChecked = false, Name = "Yellow", DisplayName = "Gelb" },
            new AccentToggles() { IsChecked = false, Name = "Brown", DisplayName = "Braun"},
            new AccentToggles() { IsChecked = false, Name = "Olive", DisplayName = "Olivegrün"},
            new AccentToggles() { IsChecked = false, Name = "Steel", DisplayName = "Stahlblau"},
            new AccentToggles() { IsChecked = false, Name = "Mauve", DisplayName = "Malve" },
            new AccentToggles() { IsChecked = false, Name = "Taupe", DisplayName = "Taupe"},
            new AccentToggles() { IsChecked = false, Name = "Sienna", DisplayName = "Ocker"}
        };

        public List<AccentToggles> ThemeList { get; } = new List<AccentToggles>()
        {
            new AccentToggles() {  IsChecked = false, Name = "BaseLight", DisplayName = "Heller Hintergrund" },
            new AccentToggles() {  IsChecked = false, Name = "BaseDark", DisplayName = "Dunkler Hintergrund" }
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

        public ICommand UpdateAccentsCommand =>
            _updateAccentsCommand ?? (_updateAccentsCommand = new RelayCommand(UpdateAccents));

        public ICommand UpdateThemmesCommand =>
            _updateThemesCommand ?? (_updateThemesCommand = new RelayCommand(UpdateThemes));

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(SaveSettings));

        private void UpdateAccents()
        {
            foreach (var accent in AccentList)
            {
                if (accent.IsChecked && accent.Equals(SelectedAccent))
                {
                    accent.IsChecked = false;
                }

                if (!accent.IsChecked || accent.Equals(SelectedAccent)) continue;

                accent.IsChecked = true;
                SelectedAccent = accent;
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

                if (!theme.IsChecked || theme.Equals(SelectedAccent)) continue;

                theme.IsChecked = true;
                SelectedTheme = theme;
            }
            SwitchStyle();
        }

        private void SwitchStyle()
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent.Name),
                ThemeManager.GetAppTheme(SelectedTheme.Name));
        }

        private void SaveSettings()
        {
            _config.SmtpAdress = SmtpAdress;
            _config.SmtpUser = Username;
        }
    }
}