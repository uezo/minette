using System;
using System.Data.SqlClient;
using Minette.Util;

namespace Minette
{
    /// <summary>
    /// Default user manager. Get/Save the user
    /// </summary>
    public class UserManager : IUserManager
    {
        /// <summary>
        /// Database name to connect
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// User table name !!CAUTION!! DO NOT SET A VALUE FROM USER INPUT
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Column of channel user ID in user table !!CAUTION!! DO NOT SET A VALUE FROM USER INPUT
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Create a user manager
        /// </summary>
        public UserManager() { }
        /// <summary>
        /// Create a user manager with the data store info
        /// </summary>
        public UserManager(string databaseName, string tableName, string userIdColumnName)
        {
            this.DatabaseName = databaseName;
            this.TableName = tableName;
            this.ColumnName = userIdColumnName;
        }

        /// <summary>
        /// Get user from data store by channel user ID or create a new user with ID
        /// </summary>
        /// <param name="channelUserId">Channel specific user ID</param>
        /// <returns>Stored or new user</returns>
        public User GetUser(string channelUserId)
        {
            if (DatabaseName == null || TableName == null || ColumnName == null)
            {
                System.Diagnostics.Debug.Print(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + "User store is not configured.");
                return new User(channelUserId);
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                var sql2 = "SELECT TOP(1) * FROM " + TableName + " WHERE " + ColumnName + " = @cuid ORDER BY Timestamp DESC";
                using (var cmd = new SqlCommand(sql2, con))
                {
                    cmd.Parameters.Add("@cuid", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@cuid"].Value = channelUserId;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            foreach (dynamic r in reader)
                            {
                                var u = new User(r);
                                u.ChannelId = r[ColumnName];
                                return u;
                            }
                        }
                    }
                }
            }
            return new User(ColumnName + "::" + channelUserId) { ChannelId = channelUserId };
        }

        /// <summary>
        /// Save user to data store
        /// </summary>
        /// <param name="session">User to save</param>
        public void SaveUser(User user)
        {
            if (DatabaseName == null || TableName == null || ColumnName == null)
            {
                return;
            }
            var ConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            using (var con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                var sql = String.Format("MERGE INTO {0} AS UT USING ( SELECT @Timestamp AS Timestamp, @Id AS Id, @Name AS Name, @NickName AS NickName, @Data AS Data, @ChannelId As ChannelId ) AS U ON ( UT.Id = U.Id ) WHEN MATCHED THEN UPDATE SET Timestamp = U.Timestamp, Name = U.Name, NickName = U.NickName, Data = U.Data WHEN NOT MATCHED THEN INSERT(Timestamp, Id, Name, NickName, Data, {1}) VALUES (U.Timestamp, U.Id, U.Name, U.NickName, U.Data, U.ChannelId);", TableName, ColumnName);
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@Timestamp", System.Data.SqlDbType.DateTime2);
                    cmd.Parameters["@Timestamp"].Value = DateTime.Now;
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Id"].Value = user.Id;
                    cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Name"].Value = user.Name;
                    cmd.Parameters.Add("@NickName", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@NickName"].Value = user.NickName;
                    cmd.Parameters.Add("@Data", System.Data.SqlDbType.NText);
                    cmd.Parameters["@Data"].Value = Json.Encode(user.Data);
                    cmd.Parameters.Add("@ChannelId", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@ChannelId"].Value = user.ChannelId;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}
