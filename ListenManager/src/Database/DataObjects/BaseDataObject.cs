using System.ComponentModel;
using System.Runtime.CompilerServices;
using ListenManager.Properties;

namespace ListenManager.Database.DataObjects
{
    public class BaseDataObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
