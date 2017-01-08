using System;
using Minette.Message;
using System.Data.SqlClient;

namespace Minette.Application
{
    public class MessageLogger : IMessageLogger
    {
        public DateTime Timestamp { get; set; }
        public string Channel { get; set; }
        public int TotalTick { get; set; }
        public int StartTick { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public EventType EventType { get; set; }
        public MessageType MessageType { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// Time zone for timestamp
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }
        public MessageLogger(string Channel)
        {
            this.StartTick = Environment.TickCount;
            this.Channel = Channel;
            this.TotalTick = 0;
            this.Timestamp = DateTime.Now;
            this.UserId = "";
            this.UserName = "";
            this.EventType = EventType.Message;
            this.MessageType = MessageType.Text;
            this.InputText = "";
            this.OutputText = "";
            this.TimeZone = System.TimeZoneInfo.Local;
        }
        public MessageLogger(string Channel, TimeZoneInfo timezone)
        {
            this.StartTick = Environment.TickCount;
            this.Channel = Channel;
            this.TotalTick = 0;
            this.Timestamp = DateTime.Now;
            this.UserId = "";
            this.UserName = "";
            this.EventType = EventType.Message;
            this.MessageType = MessageType.Text;
            this.InputText = "";
            this.OutputText = "";
            this.TimeZone = timezone;
        }
        public MessageLogger(string databaseName, string tableName, string channel)
        {
            this.StartTick = Environment.TickCount;
            this.Channel = channel;
            this.TotalTick = 0;
            this.Timestamp = DateTime.Now;
            this.UserId = "";
            this.UserName = "";
            this.EventType = EventType.Message;
            this.MessageType = MessageType.Text;
            this.InputText = "";
            this.OutputText = "";
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.TimeZone = System.TimeZoneInfo.Local;
        }
        public MessageLogger(string databaseName, string tableName, string channel, TimeZoneInfo timezone)
        {
            this.StartTick = Environment.TickCount;
            this.Channel = channel;
            this.TotalTick = 0;
            this.Timestamp = DateTime.Now;
            this.UserId = "";
            this.UserName = "";
            this.EventType = EventType.Message;
            this.MessageType = MessageType.Text;
            this.InputText = "";
            this.OutputText = "";
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.TimeZone = timezone;
        }
        public void Write()
        {
            this.TotalTick = Environment.TickCount - StartTick;
            if (DatabaseName == null || TableName == null)
            {
                System.Diagnostics.Debug.Print("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", Timestamp, Channel, TotalTick.ToString(), UserId, UserName, EventType, MessageType, InputText, OutputText);
                return;
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("INSERT INTO " + TableName + " VALUES (@timestamp, @channel, @totalTick, @UserId, @UserName, @EventType, @MessageType, @InputText, @OutputText)", con))
                {
                    cmd.Parameters.Add("@timestamp", System.Data.SqlDbType.DateTime2);
                    cmd.Parameters["@timestamp"].Value = this.Timestamp + this.TimeZone.BaseUtcOffset;
                    cmd.Parameters.Add("@Channel", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Channel"].Value = this.Channel;
                    cmd.Parameters.Add("@totalTick", System.Data.SqlDbType.Int);
                    cmd.Parameters["@totalTick"].Value = this.TotalTick;
                    cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = this.UserId;
                    cmd.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@UserName"].Value = this.UserName;
                    cmd.Parameters.Add("@EventType", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@EventType"].Value = this.EventType;
                    cmd.Parameters.Add("@MessageType", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@MessageType"].Value = this.MessageType;
                    cmd.Parameters.Add("@InputText", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@InputText"].Value = this.InputText;
                    cmd.Parameters.Add("@OutputText", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@OutputText"].Value = this.OutputText;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
