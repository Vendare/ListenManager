namespace ListenManager.Managers.Messages
{
    public class UpdateListMessage
    {
        public long IdOfItemToUpdate { get; set; }
        public bool ReloadAllData { get; set; }
    }
}
