using System.Collections.Generic;
using Minette.Util;

namespace Minette
{
    /// <summary>
    /// Request user
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identifier of the user
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nickname
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// Channel specific user ID like Twitter UserId
        /// </summary>
        public string ChannelId { get; set; }
        /// <summary>
        /// Stringified data with key
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// New User
        /// </summary>
        public User()
        {
            this.Id = "";
            this.Name = "";
            this.NickName = "";
            this.ChannelId = "";
            this.Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// New User with User ID
        /// </summary>
        /// <param name="Id">User ID</param>
        public User(string userId)
        {
            this.Id = userId;
            this.Name = "";
            this.NickName = "";
            this.ChannelId = "";
            this.Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Restored User from default database
        /// </summary>
        /// <param name="r">A record from user table</param>
        public User(dynamic r)
        {
            this.Id = (string)r["Id"];
            this.Name = (string)r["Name"];
            this.NickName = (string)r["NickName"];
            this.ChannelId = "";
            var d = (string)r["Data"];
            this.Data = Json.Decode<Dictionary<string, string>>(d);
            if (this.Data == null)
            {
                this.Data = new Dictionary<string, string>();
            }
        }
    }
}
