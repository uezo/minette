using System;
using System.Data.SqlClient;

namespace Minette.Application
{
    public class Logger : ILogger
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public Logger() { }
        public Logger(string databaseName, string tableName)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
        }
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
                        cmd.Parameters["@timestamp"].Value = DateTime.UtcNow.AddHours(9);
                        cmd.Parameters.Add("@datastr", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@datastr"].Value = message;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}
