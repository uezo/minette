using System;
using System.Collections.Generic;
using Minette.Message.Attachment;

namespace Minette.Message
{
    /// <summary>
    /// Request from user
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Message identifier
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// Indicates when this request was created
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Channel name the request was made
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Text of request message
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// List of annotated text
        /// </summary>
        public List<MecabNode> Words { get; set; }
        /// <summary>
        /// List of attached media
        /// </summary>
        public List<Media> Media { get; set; }
        /// <summary>
        /// Sticker
        /// </summary>
        public Sticker Sticker { get; set; }
        /// <summary>
        /// Latitude and longitude
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// User of request
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// In private conversation like Facebook or public like twitter
        /// </summary>
        public bool IsPrivate { get; set; }
        /// <summary>
        /// Channel specific data
        /// </summary>
        public Dictionary<string, string> ChannelData { get; set; }

        /// <summary>
        /// Create a new Request
        /// </summary>
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
        /// <summary>
        /// Create a new Request with message ID and channel name
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="channel">Channel Name</param>
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
