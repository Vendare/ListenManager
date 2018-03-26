namespace ListenManager.Interfaces
{
    public interface IWindowService
    {
        void ShowWindow(string windowName);
        bool? ShowDialog(string windowName);
    }
}
