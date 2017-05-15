using Proto;
using ProtoBuf;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Actors
{
    [ProtoContract]
    class ProtoParameter
    {
        [ProtoMember(1)]
        public string ParameterName { get; set; }
        [ProtoMember(2)]
        public string ParameterValue { get; set; }
    }

    [ProtoContract]
    class SqlWorkItem
    {
        [ProtoMember(1)]
        public string SqlQuery { get; set; }
        [ProtoMember(2)]
        public string SqlConnection { get; set; }
        [ProtoMember(3)]
        public int Interval { get; set; }
        [ProtoMember(4)]
        public string EndPoint { get; set; }
        [ProtoMember(5)]
        public long LastRun { get; set; }
        [ProtoMember(6)]
        public ProtoParameter[] SpParameters { get; set; }
    }

    public class SaveToDiskActor : IActor
    {
        private Dictionary<string, SqlWorkItem> _workItems;
        private string _filePath = "./savedworkItems.bin";

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;

            if (msg is Started)
            {
                StartUp();
            }
            else if (msg is SaveWorkItemToDiskCmd)
            {
                try
                {
                    var cmd = (SaveWorkItemToDiskCmd)msg;

                    var contract = new SqlWorkItem()
                    {
                        SqlQuery = cmd.ScheduleMessage.SqlQuery,
                        SqlConnection = cmd.ScheduleMessage.SqlConnection,
                        Interval = cmd.ScheduleMessage.Interval,
                        LastRun = cmd.ScheduleMessage.LastRun,
                        EndPoint = cmd.ScheduleMessage.EndPoint,
                        SpParameters = cmd.ScheduleMessage.SpParameters.Select(x => new ProtoParameter()
                        {
                            ParameterName = x.ParameterName,
                            ParameterValue = x.ParameterValue.ToString()
                        }
                        ).ToArray()
                    };

                    _workItems.Add(cmd.ScheduleMessage.Id, contract);
                    Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
            else if (msg is RemoveWorkItemFromDiskCmd)
            {
                try
                {
                    var cmd = (RemoveWorkItemFromDiskCmd)msg;
                    var workItem = _workItems[cmd.Id];

                    if (workItem != null)
                    {
                        _workItems.Remove(cmd.Id);
                        Save();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
            else if (msg is UpdateLastRunTickCmd)
            {
                try
                {
                    var cmd = (UpdateLastRunTickCmd)msg;
                    _workItems[cmd.Id].LastRun = cmd.NewValue;
                    Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }

            return Actor.Done;
        }

        private void Save()
        {
            using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Serializer.Serialize(file, _workItems);
            }
        }

        private void StartUp()
        {
            //////////
            try
            {
                if (File.Exists(_filePath))
                {
                    using (var file = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                    {
                        _workItems = Serializer.Deserialize<Dictionary<string, SqlWorkItem>>(file);
                    }

                    // STOP HERE, dumb
                    foreach (var item in _workItems)
                    {
                        var parameters = item.Value.SpParameters.Select(x => new Parameter(x.ParameterName, x.ParameterValue)).ToArray();

                        StaticActors.SchedulerActor
                            .Tell(new ScheduleWorkCmd(item.Key, item.Value.SqlQuery, item.Value.SqlConnection, item.Value.Interval, item.Value.EndPoint, parameters, item.Value.LastRun, false));
                    }
                }
                else
                {
                    using (var file = File.Create(_filePath))
                    {
                    }
                    _workItems = new Dictionary<string, SqlWorkItem>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
