using System;
using System.Collections.Generic;
using Minette.Message.Attachment;

namespace Minette.Message
{
    public class Request
    {
        public string MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Channel { get; set; }
        public string Text { get; set; }
        public List<MecabNode> Words { get; set; }
        public List<Media> Media { get; set; }
        public Sticker Sticker { get; set; }
        public Location Location { get; set; }
        public User User { get; set; }
        public bool IsPrivate { get; set; }
        public Dictionary<string, string> ChannelData { get; set; }
        public Request()
        {
            this.MessageId = "";
            this.Channel = "";
            this.Timestamp = DateTime.Now;
            this.Text = "";
            this.Words = new List<MecabNode>();
            this.Media = new List<Media>();
            this.IsPrivate = false;
            this.ChannelData = new Dictionary<string, string>();
        }
        public Request(string messageId, string channel)
        {
            this.MessageId = messageId;
            this.Channel = channel;
            this.Timestamp = DateTime.Now;
            this.Text = "";
            this.Words = new List<MecabNode>();
            this.Media = new List<Media>();
            this.ChannelData = new Dictionary<string, string>();
            if (channel == "LINE" || channel == "Facebook")
            {
                this.IsPrivate = true;
            }
        }
    }
}
