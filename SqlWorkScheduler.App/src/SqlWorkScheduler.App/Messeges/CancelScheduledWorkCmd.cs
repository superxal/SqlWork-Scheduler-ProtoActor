using System;
namespace SqlWorkScheduler.App.Messeges
{
    public class CancelScheduledWorkCmd
    {
        public string Id { get; private set; }

        public CancelScheduledWorkCmd(string id)
        {
            Id = id;
        }
    }
}
