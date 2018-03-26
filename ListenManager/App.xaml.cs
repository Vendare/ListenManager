using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
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
            // get the current app style (theme and accent) from the application
            // you can then use the current theme and custom accent instead set a new theme

            var appStyle = ThemeManager.DetectAppStyle(Current);
            
            ThemeManager.ChangeAppStyle(Current,
                ThemeManager.GetAccent(detectTheme()),
                appStyle.Item1); // or appStyle.Item1

            var loc = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(loc);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            base.OnStartup(e);
        }
        private string detectTheme()
        {
            var c = SystemParameters.WindowGlassColor;

            Debug.WriteLine("Current windowColor RGB is {0},{1},{2}", c.R, c.G, c.B);

            if (c.R == 0 && c.G <= 138 && c.B == 0)
            {
                return "Emerald";
            }

            if (c.R == 0 && c.G <= 171 && c.B <= 169)
            {
                return "Teal";
            }

            if (c.R == 27 && c.G <= 161 && c.B <= 226)
            {
                return "Cyan";
            }

            if (c.R == 62 && c.G <= 101 && c.B <= 255)
            {
                return "Cobalt";
            }

            if (c.R <= 96 && c.G <= 169 && c.B <= 23)
            {
                return "Green";
            }

            if (c.R <= 100 && c.G <= 118 && c.B <= 135)
            {
                return "Steel";
            }

            if (c.R <= 106 && c.G == 0 && c.B <= 255)
            {
                return "Indigo";
            }

            if (c.R <= 109 && c.G <= 135 && c.B <= 100)
            {
                return "Olive";
            }

            if (c.R <= 118 && c.G <= 96 && c.B <= 138)
            {
                return "Mauve";
            }

            if (c.R <= 130 && c.G <= 90 && c.B <= 44)
            {
                return "Brown";
            }

            if (c.R <= 135 && c.G <= 121 && c.B <= 78)
            {
                return "Taupe";
            }

            if (c.R <= 162 && c.G <= 0 && c.B <= 37)
            {
                return "Crimson";
            }

            if (c.R <= 164 && c.G <= 196 && c.B == 0)
            {
                return "Lime";
            }

            if (c.R == 170 && c.G <= 0 && c.B <= 255)
            {
                return "Violet";
            }

            if (c.R <= 216 && c.G == 0 && c.B <= 116)
            {
                return "Magenta";
            }

            if (c.R <= 229 && c.G <= 20 && c.B == 0)
            {
                return "Red";
            }

            if (c.R <= 244 && c.G <= 114 && c.B <= 208)
            {
                return "Pink";
            }

            if (c.R <= 245 && c.G <= 153 && c.B <= 10)
            {
                return "Amber";
            }

            if (c.R == 227 && c.G <= 200 && c.B == 0)
            {
                return "Yellow";
            }

            if (c.R <= 250 && c.G <= 104 && c.B == 0)
            {
                return "Orange";
            }

            return "Cobalt";
        }
    }
}
