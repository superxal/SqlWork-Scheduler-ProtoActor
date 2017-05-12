namespace SqlWorkScheduler.App.Messeges
{
    public class SaveWorkItemToDiskCmd
    {
        public ScheduleWorkCmd ScheduleMessage { get; set; }

        public SaveWorkItemToDiskCmd(ScheduleWorkCmd scheduleMessage)
        {
            ScheduleMessage = scheduleMessage;
        }
    }
}