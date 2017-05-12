namespace SqlWorkScheduler.App.Messeges
{
    public class RemoveWorkItemFromDiskCmd
    {
        public string Id { get; private set; }

        public RemoveWorkItemFromDiskCmd(string id)
        {
            Id = id;
        }
    }
}