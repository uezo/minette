using Minette.Message;
using Minette.Message.Attachment;
using System.Collections.Generic;

namespace Minette.Channel.Facebook
{
    public class Response
    {
        public Recipient Recipient { get; set; }
        public Message Message { get; set; }

        public Response(string recipientId)
        {
            this.Recipient = new Recipient() { Id = recipientId };
            this.Message = new Message();
        }

        public void SetTextMessage(string text)
        {
            this.Message.Text = text;
        }

        public void SetConfirmMessage(string text, List<Minette.Message.Attachment.Button> buttons)
        {
            this.Message.Attachment = new Attachment(AttachmentType.template);
            this.Message.Attachment.Payload = new Payload(TemplateType.button);
            this.Message.Attachment.Payload.Text = text;
            foreach (var b in buttons)
            {
                if (b.Type == Minette.Message.ButtonType.Postback)
                {
                    this.Message.Attachment.Payload.Buttons.Add(new Button(ButtonType.postback, b.Title, b.Payload));
                }
                else
                {
                    this.Message.Attachment.Payload.Buttons.Add(new Button(ButtonType.web_url, b.Title, b.Url));
                }
            }
        }

        public void SetButtonMessage(string text, List<Template> templates)
        {
            this.Message.Attachment = new Attachment(AttachmentType.template);
            this.Message.Attachment.Payload = new Payload(TemplateType.generic);
            this.Message.Attachment.Payload.Text = text;
            foreach (var t in templates)
            {
                var elm = new PayloadElement();
                elm.Title = t.Title;
                elm.Subtitle = t.Text;
                elm.Buttons = new List<Button>();
                foreach (var b in t.Buttons)
                {
                    if (b.Type == Minette.Message.ButtonType.Postback)
                    {
                        elm.Buttons.Add(new Button(ButtonType.postback, b.Title, b.Payload));
                    }
                    else
                    {
                        elm.Buttons.Add(new Button(ButtonType.web_url, b.Title, b.Url));
                    }
                }
                this.Message.Attachment.Payload.Elements.Add(elm);
            }
        }

        public void SetListMessage(Minette.Message.Response response)
        {

        }

        public void SetQuickReplyMessage(string text, List<Minette.Message.Attachment.QuickReply> quickReplies)
        {
            this.Message.Text = text;
            this.Message.QuickReplies = quickReplies;
        }
    }

    public class Recipient
    {
        public string Id { get; set; }
    }

    public class Message
    {
        public string Text { get; set; }
        public Attachment Attachment { get; set; }
        public List<QuickReply> QuickReplies { get; set; }
    }

    public enum AttachmentType { template = 1, image, audio, video, file }
    public class Attachment
    {
        public AttachmentType Type { get; set; }
        public Payload Payload { get; set; }
        public Attachment()
        {
        }
        public Attachment(AttachmentType type)
        {
            this.Type = type;
        }
    }

    public enum TemplateType { button = 1, generic, list }
    public class Payload
    {
        public TemplateType TemplateType { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string ItemUrl { get; set; }
        public List<Button> Buttons { get; set; }
        public List<PayloadElement> Elements { get; set; }
        public Payload() { }
        public Payload(TemplateType type)
        {
            this.TemplateType = type;
        }
    }

    public class PayloadElement
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string ItemUrl { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public enum ButtonType { web_url = 1, postback }
    public class Button
    {
        public ButtonType Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Payload { get; set; }
        public Button(ButtonType type, string title, string data)
        {
            this.Type = type;
            this.Title = title;
            if (type == ButtonType.web_url)
            {
                this.Url = data;
            }
            else if (type == ButtonType.postback)
            {
                this.Payload = data;
            }
        }
    }

}
