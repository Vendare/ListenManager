using System;
using System.IO;
using System.Reflection;
using System.Windows;
using ListenManager.Config;
using MahApps.Metro;

namespace ListenManager
{
    /// <inheritdoc />
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var config = ConfigHandler.Instance;
            
            ThemeManager.ChangeAppStyle(Current,
                ThemeManager.GetAccent(config.Accent),
                ThemeManager.GetAppTheme(config.Theme)); 

            var loc = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(loc);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            base.OnStartup(e);
        }
    }
}
