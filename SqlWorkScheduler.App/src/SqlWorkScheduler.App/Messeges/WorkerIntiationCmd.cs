using System.Data.SqlClient;

namespace SqlWorkScheduler.App.Messeges
{
    public class WorkerIntiationCmd
    {
        public ScheduleWorkCmd ScheduleCommand { get; set; }

        public WorkerIntiationCmd(ScheduleWorkCmd scheduleCommand)
        {
            ScheduleCommand = scheduleCommand;
        }
    }
}