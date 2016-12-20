using System.Collections.Generic;
using Minette.Util;

namespace Minette
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string ChannelId { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public User()
        {
            this.Id = "";
            this.Name = "";
            this.NickName = "";
            this.ChannelId = "";
            this.Data = new Dictionary<string, string>();
        }

        public User(string userId)
        {
            this.Id = userId;
            this.Name = "";
            this.NickName = "";
            this.ChannelId = "";
            this.Data = new Dictionary<string, string>();
        }

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
