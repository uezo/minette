using System;
using System.Data.SqlClient;

namespace Minette.Application
{
    /// <summary>
    /// Simple logger
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Database name to connect
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// Session table name !!CAUTION!! DO NOT SET A VALUE FROM USER INPUT
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Time zone for timestamp
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }
        /// <summary>
        /// Create a new logger
        /// </summary>
        public Logger()
        {
            this.TimeZone = System.TimeZoneInfo.Local;
        }
        /// <summary>
        /// Create a new logger with timezone
        /// </summary>
        public Logger(TimeZoneInfo timezone)
        {
            this.TimeZone = timezone;
        }
        /// <summary>
        /// Create a new logger with data store info
        /// </summary>
        public Logger(string databaseName, string tableName)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.TimeZone = System.TimeZoneInfo.Local;
        }
        /// <summary>
        /// Create a new logger with data store info and timezone
        /// </summary>
        public Logger(string databaseName, string tableName, TimeZoneInfo timezone)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.TimeZone = timezone;
        }
        /// <summary>
        /// Write log message
        /// </summary>
        /// <param name="message">String to be logged</param>
        public void Write(string message)
        {
            if (DatabaseName == null || TableName == null)
            {
                System.Diagnostics.Debug.Print(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + message);
            }
            else
            {
                var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
                using (var con = new SqlConnection(ConnectionStr))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("INSERT INTO " + TableName + " VALUES (@timestamp, @datastr)", con))
                    {
                        cmd.Parameters.Add("@timestamp", System.Data.SqlDbType.DateTime2);
                        cmd.Parameters["@timestamp"].Value = DateTime.Now + this.TimeZone.BaseUtcOffset;
                        cmd.Parameters.Add("@datastr", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@datastr"].Value = message;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
