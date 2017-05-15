using Proto;
using ProtoBuf;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
//using System.Data;
using System.Xml;
using System.Data.Common;
using System.Data;
using System.Xml.Serialization;
using Microsoft.SqlServer.Server;

namespace SqlWorkScheduler.App.Actors
{
    public class WorkPerformerActor : IActor
    {
        private string _lastRunReplacement = "{lastRun}";
        private ScheduleWorkCmd _cmd;
        private long _lastRun;

        public Task ReceiveAsync(IContext context)
        {
            context.SetBehavior(ReceiveNotInitiated);
            return Actor.Done;
        }

        #region Behaviors
        public Task ReceiveNotInitiated(IContext context)
        {
            var msg = context.Message;

            if (msg is WorkerIntiationCmd)
            {
                IntiateWorker((WorkerIntiationCmd)msg);
            }

            context.SetBehavior(ReceiveIntiated);
            return Actor.Done;
        }

        public Task ReceiveIntiated(IContext context)
        {
            var msg = context.Message;

            if (msg is PerformWorkCmd)
            {
                PerformWork();
            }

            return Actor.Done;
        }
        #endregion

        private void IntiateWorker(WorkerIntiationCmd cmd)
        {
            _lastRun = cmd.ScheduleCommand.LastRun;
            _cmd = cmd.ScheduleCommand;
        }

        private async void PerformWork()
        {
            try
            {
                using (var sqlClient = new SqlConnection(_cmd.SqlConnection))
                {
                    sqlClient.Open();

                    string sqlQuery;
                    if (_cmd.LastRun == 0)
                    {
                        sqlQuery = _cmd.SqlQuery.Replace(_lastRunReplacement, "'" + SqlDateTime.MinValue.ToSqlString().ToString() + "'");
                    }
                    else
                    {
                        var temp = new DateTime().AddTicks(_lastRun);
                        var date = "'" + new SqlDateTime(temp).ToSqlString().ToString() + "'";

                        sqlQuery = _cmd.SqlQuery.Replace(_lastRunReplacement, date);
                    }


                    using (var sqlCommand = new SqlCommand(sqlQuery, sqlClient))
                    {
                        if (_cmd.SpParameters != null)
                        {
                            if (_cmd.SpParameters.Length > 0)
                            {
                                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                                foreach (var parameter in _cmd.SpParameters)
                                {
                                    sqlCommand.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.ParameterValue));
                                }
                            }
                        }

                        // web request
                        var request = WebRequest.CreateHttp(_cmd.EndPoint);
                        request.Method = "POST";

                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            //reader.Get
                            using(var webStream = await request.GetRequestStreamAsync())
                            {
                                Serializer.
                            }
                        }
                    }

                    sqlClient.Close();
                }

                _lastRun = DateTime.Now.Ticks;
                StaticActors.SaveToDiskActor
                    .Tell(new UpdateLastRunTickCmd(_cmd.Id, _lastRun));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
