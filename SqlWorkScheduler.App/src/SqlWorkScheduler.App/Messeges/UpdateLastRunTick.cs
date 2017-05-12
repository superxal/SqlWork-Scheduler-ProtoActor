namespace SqlWorkScheduler.App.Messeges
{
    public class UpdateLastRunTickCmd
    {
        public string Id { get; private set; }
        public long NewValue { get; private set; }

        public UpdateLastRunTickCmd(string id, long newValue)
        {
            Id = id;
            NewValue = newValue;
        }
    }
}