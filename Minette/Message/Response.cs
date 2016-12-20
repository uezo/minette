using System.Collections.Generic;
using Minette.Message.Attachment;

namespace Minette.Message
{
    public enum ResponseType { Text = 1, Image, Video, Audio, Sticker, Template, QuickReply }
    public class Response
    {
        public ResponseType Type { get; set; }
        public string MessageId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string PreviewImageUrl { get; set; }
        public Sticker Sticker { get; set; }
        public List<Template> Templates { get; set; }
        public List<QuickReply> QuickReplies { get; set; }

        public Response(string messageId, ResponseType type)
        {
            this.MessageId = messageId;
            this.Type = type;
            if (type == ResponseType.Sticker)
            {
                this.Sticker = new Sticker();
            }
            else if (type == ResponseType.Template)
            {
                this.Templates = new List<Template>();
            }
            else if (type == ResponseType.QuickReply)
            {
                this.QuickReplies = new List<QuickReply>();
            }
        }
        public Response(string messageId, string text)
        {
            this.Type = ResponseType.Text;
            this.MessageId = messageId;
            this.Text = text;
        }
        public Response(string messageId, string imageUrl, string previewUrl)
        {
            this.Type = ResponseType.Image;
            this.MessageId = messageId;
            this.ImageUrl = imageUrl;
            this.PreviewImageUrl = previewUrl;
        }
        public void SetConfirmTemplate(string title, string text, string yesLabel, string yesData, string noLabel, string noData)
        {
            var t = new Template(TemplateType.Confirm);
            t.Title = title;
            t.Text = text;
            t.AddButton(yesLabel, yesData);
            t.AddButton(noLabel, noData);
            this.Templates.Add(t);
        }
    }

}
