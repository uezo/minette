using Minette.Message;
using System.Collections.Generic;

namespace Minette.Channel.Line
{
    public class Response
    {
        public string replyToken { get; set; }
        public List<SendMessage> messages { get; set; }
        public Response(string replyToken)
        {
            this.replyToken = replyToken;
            this.messages = new List<SendMessage>();
        }
        public void addTextMessage(string text)
        {
            var msg = new SendMessage();
            msg.type = "text";
            msg.text = text;
            this.messages.Add(msg);
        }
        public void AddImageMessage(string mediaUrl, string thumbUrl)
        {
            var msg = new SendMessage();
            msg.type = "image";
            msg.originalContentUrl = mediaUrl;
            msg.previewImageUrl = thumbUrl;
            this.messages.Add(msg);
        }
        public void AddVideoMessage(string mediaUrl, string thumbUrl)
        {
            var msg = new SendMessage();
            msg.type = "video";
            msg.originalContentUrl = mediaUrl;
            msg.previewImageUrl = thumbUrl;
            this.messages.Add(msg);
        }
        public void AddAudioMessage(string mediaUrl, int duration)
        {
            var msg = new SendMessage();
            msg.type = "audio";
            msg.originalContentUrl = mediaUrl;
            msg.duration = duration;
            this.messages.Add(msg);
        }
        public void AddLocationMessage(string title, string address, decimal latitude, decimal longitude)
        {
            var msg = new SendMessage();
            msg.type = "location";
            msg.title = title;
            msg.address = address;
            msg.latitude = latitude;
            msg.longitude = longitude;
            this.messages.Add(msg);
        }
        public void AddStickerMessage(string packageId, string stickerId)
        {
            var msg = new SendMessage();
            msg.type = "sticker";
            msg.packageId = packageId;
            msg.stickerId = stickerId;
            this.messages.Add(msg);
        }
        public void AddImagemapMessage(string altText, string baseUrl, int heightPercentage, List<Action> actions)
        {
            var msg = new SendMessage();
            msg.type = "imagemap";
            msg.altText = altText;
            msg.baseUrl = baseUrl;
            msg.baseSize = new BaseSize(1040 * heightPercentage / 100);
            msg.actions = actions;
            this.messages.Add(msg);
        }
        public void AddButtonMessage(string text, string altText, string title, string thumbUrl, List<TemplateAction> actions)
        {
            var msg = new SendMessage();
            msg.type = "template";
            msg.altText = altText;
            var template = new ButtonTemplate();
            template.text = text;
            template.title = title;
            template.thumbnailImageUrl = thumbUrl;
            template.actions = actions;
            msg.template = template;
            this.messages.Add(msg);
        }
        public void AddButtonMessage(Minette.Message.Response response)
        {
            var actions = new List<TemplateAction>();
            foreach (var b in response.Templates[0].Buttons)
            {
                actions.Add(new TemplateAction(b));
            }
            AddButtonMessage(response.Templates[0].Text, response.Templates[0].Text, response.Templates[0].Title, null, actions);
        }
        public void AddConfirmMessage(string text, string altText, ButtonType yesType, string yesLabel, string yesData, ButtonType noType, string noLabel, string noData)
        {
            var msg = new SendMessage();
            msg.type = "template";
            msg.altText = altText;
            var template = new ConfirmTemplate();
            template.text = text;
            var yesAction = new Action() { label = yesLabel };
            if(yesType == ButtonType.Postback)
            {
                yesAction.type = "postback";
                yesAction.data = yesData;
            }
            else if (yesType == ButtonType.WebUrl)
            {
                yesAction.type = "uri";
                yesAction.uri = yesData;
            }
            else
            {
                yesAction.type = "message";
                yesAction.text = yesData;
            }
            var noAction = new Action() { label = noLabel };
            if (noType == ButtonType.Postback)
            {
                noAction.type = "postback";
                noAction.data = noData;
            }
            else if (noType == ButtonType.WebUrl)
            {
                noAction.type = "uri";
                noAction.uri = noData;
            }
            else
            {
                noAction.type = "message";
                noAction.text = noData;
            }
            template.actions.Add(yesAction);
            template.actions.Add(noAction);
            msg.template = template;
            this.messages.Add(msg);
        }
        public void AddConfirmMessage(Minette.Message.Response response)
        {
            var btns = response.Templates[0].Buttons;
            AddConfirmMessage(
            response.Templates[0].Text, response.Templates[0].Text,
                btns[0].Type, btns[0].Title, btns[0].Type == ButtonType.WebUrl ? btns[0].Url : btns[0].Payload,
                btns[1].Type, btns[1].Title, btns[1].Type == ButtonType.WebUrl ? btns[1].Url : btns[1].Payload
            );
        }
        public void AddCarouselMessage(string altText, List<Column> columns)
        {
            var msg = new SendMessage();
            msg.type = "template";
            msg.altText = altText;
            var template = new CarouselTemplate();
            template.columns = columns;
            msg.template = template;
            this.messages.Add(msg);
        }
        public void AddCarouselMessage(Minette.Message.Response response)
        {
            var columns = new List<Column>();
            foreach (var t in response.Templates)
            {
                var actions = new List<TemplateAction>();
                foreach (var b in t.Buttons)
                {
                    actions.Add(new TemplateAction(b));
                }
                var c = new Column();
                c.actions = actions;
                c.title = t.Title;
                c.text = t.Text;
                columns.Add(c);
            }
            AddCarouselMessage(response.Templates[0].Text, columns);
        }
    }
    public class SendMessage
    {
        public string type { get; set; }
        public string text { get; set; }
        //media
        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }
        //audio
        public int duration { get; set; }
        //location
        public string title { get; set; }
        public string address { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        //sticker
        public string packageId { get; set; }
        public string stickerId { get; set; }
        //rich(imagemap & template)
        public string baseUrl { get; set; }
        public string altText { get; set; }
        public BaseSize baseSize { get; set; }
        public List<Action> actions { get; set; }
        public Template template { get; set; }
    }
    public class BaseSize
    {
        public int height { get; set; }
        public int width { get; set; }
        public BaseSize()
        {
            this.height = 1040;
            this.width = 1040;
        }
        public BaseSize(int height)
        {
            this.height = height;
            this.width = 1040;
        }
    }
    public class Action
    {
        public string type { get; set; }
        public string label { get; set; }
        public string text { get; set; }
        public string data { get; set; }
        public string uri { get; set; }
        public string linkUri { get; set; }
        public ImageapArea area { get; set; }
        public Action() { }
        public Action(string type)
        {
            this.type = type;
        }
        public Action(string linkUri, int x, int y, int width, int height)
        {
            this.type = "uri";
            this.linkUri = linkUri;
            this.area = new ImageapArea();
            this.area.x = x;
            this.area.y = y;
            this.area.width = width;
            this.area.height = height;
        }
    }
    public class ImageapArea
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public ImageapArea() { }
        public ImageapArea(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
    public class Template
    {
        public string type { get; set; }
    }
    public class ButtonTemplate : Template
    {
        public string thumbnailImageUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public List<TemplateAction> actions { get; set; }
        public ButtonTemplate()
        {
            this.type = "buttons";
            this.actions = new List<TemplateAction>();
        }
    }
    public class ConfirmTemplate : Template
    {
        public string text { get; set; }
        public List<Action> actions { get; set; }
        public ConfirmTemplate()
        {
            this.type = "confirm";
            this.actions = new List<Action>();
        }
    }
    public class CarouselTemplate : Template
    {
        public List<Column> columns { get; set; }
        public CarouselTemplate()
        {
            this.type = "carousel";
            this.columns = new List<Column>();
        }
    }
    public class Column
    {
        public string thumbnailImageUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public List<TemplateAction> actions { get; set; }
        public Column()
        {
            this.actions = new List<TemplateAction>();
        }
    }
    public class TemplateAction
    {
        public string type { get; set; }
        public string label { get; set; }
        public string text { get; set; }
        public string data { get; set; }
        public string uri { get; set; }
        public TemplateAction() { }
        public TemplateAction(Minette.Message.Attachment.Button b)
        {
            this.label = b.Title;
            if (b.Type == ButtonType.Postback)
            {
                this.type = "postback";
                this.data = b.Payload;
            }
            else if (b.Type == ButtonType.WebUrl)
            {
                this.type = "uri";
                this.uri = b.Url;
            }
            else
            {
                this.type = "message";
                this.text = b.Payload;
            }
        }
    }
}
