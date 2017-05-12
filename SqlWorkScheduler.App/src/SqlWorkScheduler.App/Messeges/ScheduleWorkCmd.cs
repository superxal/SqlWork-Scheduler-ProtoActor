using System.Collections.Generic;

namespace SqlWorkScheduler.App.Messeges
{
    public class ScheduleWorkCmd
    {
        public string Id { get; private set; }
        public string SqlQuery { get; private set; }
        public string SqlConnection { get; private set; }
        public int Interval { get; private set; }
        public string EndPoint { get; private set; }
        public Dictionary<string, string> SpParameters { get; set; }
        public bool SaveToDisk { get; private set; }
        public long LastRun { get; private set; }

        public ScheduleWorkCmd(string id, string sqlQuery, string sqlConnection, int interval, string endpoint, Dictionary<string, string> spParameters = null, long lastRun = 0, bool saveToDisk = true)
        {
            Id = id;
            SqlQuery = sqlQuery;
            SqlConnection = sqlConnection;
            Interval = interval;
            EndPoint = endpoint;
            SaveToDisk = saveToDisk;
            LastRun = lastRun;
            SpParameters = spParameters;
        }
    }

    //class ScheduleStoredProcedure
    //{
    //    public string Id { get; private set; }
    //    public string StoredProcedureName { get; set; }
    //    public string SqlConnection { get; private set; }
    //    public int Interval { get; private set; }
    //    public string EndPoint { get; private set; }
    //    public bool SaveToDisk { get; private set; }
    //    public long LastRun { get; private set; }
    //    //public SqlParameter[] Parameters { get; set; }

    //    public ScheduleStoredProcedure(string id, string storedProcedureName, SqlParameter[] parameters, string sqlConnection, int interval, string endpoint, long lastRun = 0, bool saveToDisk = true)
    //    {
    //        Id = id;
    //        StoredProcedureName = storedProcedureName;
    //        SqlConnection = sqlConnection;
    //        Interval = interval;
    //        EndPoint = endpoint;
    //        SaveToDisk = saveToDisk;
    //        LastRun = lastRun;
    //    }
    //}
}
