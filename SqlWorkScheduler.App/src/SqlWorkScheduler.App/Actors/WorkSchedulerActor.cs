using Proto;
using SqlWorkScheduler.App.Messeges;
using System;
using Proto.Schedulers.SimpleScheduler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SqlWorkScheduler.App.Actors
{
    class WorkDescription
    {
        //public ScheduleWorkCmd Command { get; set; }
        public PID ActorPID { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }

    public class WorkSchedulerActor : IActor
    {
        private ISimpleScheduler _scheduler = new SimpleScheduler();
        private Dictionary<string, WorkDescription> _scheduledWork = new Dictionary<string, WorkDescription>();

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;

            if (msg is ScheduleWorkCmd)
            {
                try
                {
                    var cmd = (ScheduleWorkCmd)msg;

                    var pid = Actor.Spawn(Actor.FromProducer(() => new WorkPerformerActor()));
                    pid.Tell(new WorkerIntiationCmd(cmd));
                    CancellationTokenSource tokenSource;
                    _scheduler.ScheduleTellRepeatedly(
                        TimeSpan.Zero,
                        TimeSpan.FromMinutes(cmd.Interval),
                        pid,
                        new PerformWorkCmd(),
                        out tokenSource
                    );

                    if(cmd.SaveToDisk)
                    {
                        StaticActors.SaveToDiskActor
                            .Tell(new SaveWorkItemToDiskCmd(cmd));
                    }

                    _scheduledWork.Add(cmd.Id, new WorkDescription()
                    {
                        ActorPID = pid,
                        TokenSource = tokenSource
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
            else if (msg is CancelScheduledWorkCmd)
            {
                var cmd = (CancelScheduledWorkCmd)msg;

                try
                {
                    var description = _scheduledWork[cmd.Id];

                    if (description != null)
                    {
                        description.TokenSource.Cancel();
                        description.ActorPID.Stop();

                        StaticActors.SaveToDiskActor.Tell(new RemoveWorkItemFromDiskCmd(cmd.Id));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }

            return Actor.Done;
        }
    }
}
