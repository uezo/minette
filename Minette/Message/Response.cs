using System.Collections.Generic;
using Minette.Message.Attachment;

namespace Minette.Message
{
    public enum ResponseType { Text = 1, Image, Video, Audio, Sticker, Template, QuickReply }
    /// <summary>
    /// Response to user
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Type of response. Text, Image, Video, Audio, Sticker, Template or QuickReply.
        /// </summary>
        public ResponseType Type { get; set; }
        /// <summary>
        /// Identifier of response
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// Body text
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Url of attached image
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Url of attached image for preview
        /// </summary>
        public string PreviewImageUrl { get; set; }
        /// <summary>
        /// Sticker info
        /// </summary>
        public Sticker Sticker { get; set; }
        /// <summary>
        /// List of template applied messages
        /// </summary>
        public List<Template> Templates { get; set; }
        /// <summary>
        /// List of QuickReplies
        /// </summary>
        public List<QuickReply> QuickReplies { get; set; }

        /// <summary>
        /// Create a new Response with message ID and the type of response
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="type">Response type</param>
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
        /// <summary>
        /// Create a new Text Response with message ID and the body
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="text">Body text</param>
        public Response(string messageId, string text)
        {
            this.Type = ResponseType.Text;
            this.MessageId = messageId;
            this.Text = text;
        }
        /// <summary>
        /// Create a new Image Response with message ID and the urls
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="imageUrl">Url of image</param>
        /// <param name="previewUrl">Url of image for preview</param>
        public Response(string messageId, string imageUrl, string previewUrl)
        {
            this.Type = ResponseType.Image;
            this.MessageId = messageId;
            this.ImageUrl = imageUrl;
            this.PreviewImageUrl = previewUrl;
        }
        /// <summary>
        /// Set confirmation buttons to Response
        /// </summary>
        /// <param name="title">Title of confirmation message box</param>
        /// <param name="text">Body text of confirmation message box</param>
        /// <param name="yesType">Type for YES button</param>
        /// <param name="yesLabel">Label for YES button</param>
        /// <param name="yesData">Data for YES button</param>
        /// <param name="noType">Type for NO button</param>
        /// <param name="noLabel">Label for NO button</param>
        /// <param name="noData">Data for NO button</param>
        public void SetConfirmTemplate(string title, string text, ButtonType yesType, string yesLabel, string yesData, ButtonType noType, string noLabel, string noData)
        {
            var t = new Template(TemplateType.Confirm);
            t.Title = title;
            t.Text = text;
            t.Buttons.Add(new Button(yesType, yesLabel, yesData));
            t.Buttons.Add(new Button(noType, noLabel, noData));
            if (this.Templates == null)
            {
                this.Templates = new List<Template>();
            }
            else
            {
                this.Templates.Clear();
            }
            this.Templates.Add(t);
        }
        /// <summary>
        /// Set buttons to Response
        /// </summary>
        /// <param name="title">Title of confirmation message box</param>
        /// <param name="text">Body text of confirmation message box</param>
        /// <param name="buttons">List of buttons you want to add</param>
        public void SetButtonTemplate(string title, string text, List<Button> buttons)
        {
            var t = new Template(TemplateType.Button);
            t.Title = title;
            t.Text = text;
            t.Buttons = buttons;
            if(this.Templates == null)
            {
                this.Templates = new List<Template>();
            }
            this.Templates.Add(t);
        }
    }

}
