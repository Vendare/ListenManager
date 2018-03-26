using System.ComponentModel;
using System.Runtime.CompilerServices;
using ListenManager.Annotations;

namespace ListenManager.Managers
{
    public class BaseManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
