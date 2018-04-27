using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.Handlers;
using ListenManager.Managers.Messages;

namespace ListenManager.Managers
{
    internal class OverviewManager : BaseManager
    {
        private readonly VerzeichnisHandler _handler;

       public OverviewManager()
        {
            _handler = VerzeichnisHandler.Instance;
            Mitglieder = _handler.GetAllMitglieder();
            Messenger.Default.Register<ReloadMemberMessage>(this, OnHandleReloadMessage);
        }

        private void OnHandleReloadMessage(ReloadMemberMessage msg)
        {
            if (msg.ReloadAllMembers)
            {
                Mitglieder = _handler.GetAllMitglieder();
            }
        }
    }
}
