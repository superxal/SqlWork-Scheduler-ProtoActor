using Proto;
using SqlWorkScheduler.App.Actors;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App
{
    class StaticActors
    {
        public static PID SchedulerActor { get; set; }
        public static PID SaveToDiskActor { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var props = Actor.FromProducer(() => new WorkSchedulerActor());
            StaticActors.SchedulerActor = Actor.Spawn(props);

            props = Actor.FromProducer(() => new SaveToDiskActor());
            StaticActors.SaveToDiskActor = Actor.Spawn(props);

            var connectionString = "Server=.\\;Initial Catalog=NORTHWND;Integrated Security=true";
            var parameters = new Parameter[] {
                        new Parameter("@CategoryName", "Beverages"),
                        new Parameter("@OrdYear", "1998")
                    };

            StaticActors.SchedulerActor.Tell(
                new ScheduleWorkCmd(
                    Guid.NewGuid().ToString(),
                    "SalesByCategory",
                    connectionString,
                    5,
                    "http://localhost:3550/",
                    parameters
                ));

            //StaticActors.SchedulerActor.Tell(
            //    new ScheduleWorkCmd(
            //        Guid.NewGuid().ToString(),
            //        "select * from Orders",
            //        connectionString,
            //        1,
            //        "http://localhost:3550/"
            //    ));

            Console.ReadLine();
        }
    }
}
