using System;
using System.Data.SqlClient;
using Minette.Util;

namespace Minette
{
    public class SessionManager : ISessionManager
    {
        //メンバ変数
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        //コンストラクタ
        public SessionManager() { }
        public SessionManager(string databaseName, string tableName)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
        }
        //セッションの取得
        public Session GetSession(string Id)
        {
            if (DatabaseName == null || TableName == null)
            {
                System.Diagnostics.Debug.Print(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + "Session store is not configured.");
                return new Session(Id);
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
                                if (DateTime.UtcNow.AddHours(9) > ((DateTime)r["Timestamp"]).AddMinutes(5))
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
        //セッションの保存
        public void SaveSession(Session session)
        {
            if (DatabaseName == null || TableName == null)
            {
                return;
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("INSERT INTO " + TableName + "(Timestamp, Id, Mode, DialogStatus, ChatContext, Data) VALUES (@Timestamp, @Id, @Mode, @DialogStatus, @ChatContext, @Data)", con))
                {
                    cmd.Parameters.Add("@Timestamp", System.Data.SqlDbType.DateTime2);
                    cmd.Parameters["@Timestamp"].Value = DateTime.UtcNow.AddHours(9);
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
