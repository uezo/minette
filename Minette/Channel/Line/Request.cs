using System;
using System.Collections.Generic;
using Minette.Message;

namespace Minette.Channel.Line
{
    public class Request
    {
        public List<WebhookEvent> Events { get; set; }
        public Request()
        {
            this.Events = new List<WebhookEvent>();
        }
        public Request(dynamic json)
        {
            this.Events = new List<WebhookEvent>();
            if (json.events != null)
            {
                foreach (var ev in json.events)
                {
                    this.Events.Add(new WebhookEvent(ev));
                }
            }
            else
            {
                this.Events.Add(new WebhookEvent(json));
            }
        }
    }

    public class WebhookEvent
    {
        public EventType Type { get; set; }
        public string ReplyToken { get; set; }
        public DateTime Timestamp { get; set; }
        public Source Source { get; set; }
        public Message Message { get; set; }
        //public Postback Postback { get; set; }    //未使用
        //public Beacon Beacon { get; set; }        //未使用
        public WebhookEvent(dynamic json)
        {
            this.Timestamp = new DateTime((long)json.timestamp);
            this.Source = new Source(json.source);
            if (json.type == "message")
            {
                this.Type = EventType.Message;
                this.ReplyToken = json.replyToken;
                this.Message = new Message(json.message);
            }
            else if (json.type == "follow")
            {
                this.Type = EventType.Follow;
                this.ReplyToken = json.replyToken;
            }
            else if (json.type == "unfollow")
            {
                this.Type = EventType.Unfollow;
            }
            else if (json.type == "join")
            {
                this.Type = EventType.Join;
                this.ReplyToken = json.replyToken;
            }
            else if (json.type == "leave")
            {
                this.Type = EventType.Leave;
            }
            else if (json.type == "postback")
            {
                this.Type = EventType.Postback;
                this.ReplyToken = json.replyToken;
            }
            else if (json.type == "beacon")
            {
                this.Type = EventType.Beacon;
                this.ReplyToken = json.replyToken;
            }
        }
    }
    public enum SourceType { User = 1, Group, Room };
    public class Source
    {
        public SourceType Type { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string RoomId { get; set; }
        public Source(dynamic json)
        {
            if (json.type == "user")
            {
                this.Type = SourceType.User;
                this.UserId = json.userId;
            }
            else if (json.type == "group")
            {
                this.Type = SourceType.Group;
                this.UserId = json.groupId;
            }
            else if (json.type == "room")
            {
                this.Type = SourceType.Room;
                this.UserId = json.roomId;
            }
        }

    }
    public class Message
    {
        public MessageType Type { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PackageId { get; set; }
        public string StickerId { get; set; }
        public byte[] BinaryData { get; set; }
        public Message(dynamic json)
        {
            if (json.type == "text")
            {
                this.Type = MessageType.Text;
                this.Id = json.id;
                this.Text = json.text;
            }
            else if (json.type == "image")
            {
                this.Type = MessageType.Image;
                this.Id = json.id;
            }
            else if (json.type == "video")
            {
                this.Type = MessageType.Video;
                this.Id = json.id;
            }
            else if (json.type == "audio")
            {
                this.Type = MessageType.Audio;
                this.Id = json.id;
            }
            else if (json.type == "location")
            {
                this.Type = MessageType.Location;
                this.Id = json.id;
                this.Title = json.title;
                this.Address = json.address;
                this.Latitude = json.latitude;
                this.Longitude = json.longitude;
            }
            else if (json.type == "sticker")
            {
                this.Type = MessageType.Sticker;
                this.Id = json.id;
                this.PackageId = json.packageId;
                this.StickerId = json.stickerId;
            }

        }
    }
    //public class Postback
    //{
    //    public string Data { get; set; }
    //    public Postback(dynamic json)
    //    {
    //        this.Data = json.data;
    //    }
    //}
    //public class Beacon
    //{
    //    public string Hwid { get; set; }
    //    public string Type { get; set; }
    //    public Beacon(dynamic json)
    //    {
    //        this.Hwid = json.hwid;
    //        this.Type = "enter";
    //    }
    //}
}
