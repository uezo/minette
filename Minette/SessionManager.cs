using System;
using System.Data.SqlClient;
using Minette.Util;
using System.Collections.Generic;

namespace Minette
{
    /// <summary>
    /// Default session manager. Get/Save the session
    /// </summary>
    public class SessionManager : ISessionManager
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
        /// Session timeout (seconds)
        /// </summary>
        public int Timeout { get; set; }
        private Dictionary<string, KeyValuePair<DateTime, Session>> InMemoryStore { get; set; }

        /// <summary>
        /// Create a new InMemory Session Manager(Timeout=300sec)
        /// </summary>
        public SessionManager() {
            this.InMemoryStore = new Dictionary<string, KeyValuePair<DateTime, Session>>();
            this.Timeout = 300;
        }
        /// <summary>
        /// Create a new InMemory Session Manager
        /// </summary>
        /// <param name="timeout">Timeout after this value(seconds)</param>
        public SessionManager(int timeout)
        {
            this.InMemoryStore = new Dictionary<string, KeyValuePair<DateTime, Session>>();
            this.Timeout = timeout;
        }
        /// <summary>
        /// Create a new Database Session Manager
        /// </summary>
        /// <param name="databaseName">Database to connect</param>
        /// <param name="tableName">Session table name</param>
        public SessionManager(string databaseName, string tableName)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.Timeout = 300;
        }
        /// <summary>
        /// Create a new Database Session Manager
        /// </summary>
        /// <param name="databaseName">Database to connect</param>
        /// <param name="tableName">Session table name</param>
        /// <param name="timeout">Timeout after this value(seconds)</param>
        public SessionManager(string databaseName, string tableName, int timeout)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Get session from data store by session ID or create a new session with ID
        /// </summary>
        /// <param name="Id">Key for session</param>
        /// <returns>Stored or new session</returns>
        public Session GetSession(string Id)
        {
            if (DatabaseName == null || TableName == null)
            {
                var sess = InMemoryStore.ContainsKey(Id) && DateTime.Now <= InMemoryStore[Id].Key.AddSeconds(Timeout) ? InMemoryStore[Id].Value : new Session(Id);
                sess.KeepMode = false;
                sess.DialogService = new Minette.Application.DialogService();
                if(sess.Mode != "")
                {
                    sess.ModeStatus = ModeStatus.Continue;
                }
                else
                {
                    sess.ModeStatus = ModeStatus.Start;
                }
                return sess;
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                var sql = "SELECT TOP(1) * FROM " + TableName + " WHERE Id = @SessionId ORDER BY TIMESTAMP DESC";
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@SessionId", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@SessionId"].Value = Id;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            foreach (dynamic r in reader)
                            {
                                if (DateTime.Now > ((DateTime)r["Timestamp"]).AddSeconds(Timeout))
                                {
                                    break;
                                }
                                else
                                {
                                    return new Session(r);
                                }
                            }
                        }
                    }
                }
            }
            return new Session(Id);
        }

        /// <summary>
        /// Save session to data store
        /// </summary>
        /// <param name="session">Session to save</param>
        public void SaveSession(Session session)
        {
            if (DatabaseName == null || TableName == null)
            {
                InMemoryStore[session.Id] = new KeyValuePair<DateTime, Session>(DateTime.Now, session);
                return;
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("INSERT INTO " + TableName + "(Timestamp, Id, Mode, DialogStatus, ChatContext, Data) VALUES (@Timestamp, @Id, @Mode, @DialogStatus, @ChatContext, @Data)", con))
                {
                    cmd.Parameters.Add("@Timestamp", System.Data.SqlDbType.DateTime2);
                    cmd.Parameters["@Timestamp"].Value = DateTime.Now;
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Id"].Value = session.Id;
                    cmd.Parameters.Add("@Mode", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Mode"].Value = session.Mode;
                    cmd.Parameters.Add("@DialogStatus", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@DialogStatus"].Value = session.DialogStatus;
                    cmd.Parameters.Add("@ChatContext", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@ChatContext"].Value = session.ChatContext;
                    cmd.Parameters.Add("@Data", System.Data.SqlDbType.NText);
                    cmd.Parameters["@Data"].Value = Json.Encode(session.Data);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
