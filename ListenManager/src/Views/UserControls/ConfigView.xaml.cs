using System.Windows.Controls;
using ListenManager.Config;

namespace ListenManager.Views.UserControls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Interaktionslogik für SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        //For Security no MVVM aproach here
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            var config = ConfigHandler.Instance;

            config.SmtpPassword = PasswordBox.SecurePassword;
            if (btn?.Command == null || !btn.Command.CanExecute(btn.CommandParameter)) return;
            btn.Command.Execute(btn.CommandParameter);
        }
    }
}
