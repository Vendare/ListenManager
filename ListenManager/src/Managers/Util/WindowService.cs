using ListenManager.Interfaces;
using MahApps.Metro.Controls;
using ListenManager.Views.Pages;

namespace ListenManager.Managers.Util
{
    public class WindowService : IWindowService
    {
        public bool? ShowDialog(string windowName)
        {
            var win = DetermineWindow(windowName);
            return win?.ShowDialog();
        }

        public void ShowWindow(string windowName)
        {
            var win = DetermineWindow(windowName);
            win?.Show();
        }

        private MetroWindow DetermineWindow(string windowName)
        {
            if (windowName.Equals(nameof(EditMemberPage)))
            {
                return new EditMemberPage();
            }

            if (windowName.Equals(nameof(EditListPage)))
            {
                return new EditListPage();
            }

            return null;
        }
    }
}
