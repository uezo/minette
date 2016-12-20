//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using Minette.Util;
//using Minette.Message;
//using Minette.Message.Attachment;
//using Minette.Application;

//namespace Minette.Connector
//{
//    public class Line
//    {
//        public Minette.Core MinetteCore { get; set; }
//        public IUserManager UserManager { get; set; }
//        public IMessageLogger MessageLogger { get; set; }
//        public string ChannelAccessToken { get; set; }

//        public Line()
//        {
//            this.MinetteCore = new Minette.Core();
//            this.UserManager = new UserManager();
//            this.MessageLogger = new MessageLogger("LINE");
//        }
//        public Line(Minette.Core minetteCore, IUserManager userManager, IMessageLogger messageLogger, string channelAccessToken)
//        {
//            this.MinetteCore = minetteCore;
//            this.UserManager = userManager;
//            this.MessageLogger = messageLogger;
//            this.ChannelAccessToken = channelAccessToken;
//        }

//        public async Task ProcessRequestAsync(CancellationToken cancellationToken, string decodedString)
//        {
//            try
//            {
//                //リクエストのセットアップ
//                var linereq = new LineRequest((Json.Decode(decodedString)).events[0]).Events[0];
//                var req = new Request(linereq.Message.Id, "LINE");
//                //テキスト
//                if (linereq.Message.Type == MessageType.Text)
//                {
//                    req.Text = linereq.Message.Text;
//                }
//                //画像
//                else if (linereq.Message.Type == MessageType.Image)
//                {
//                    var m = new Media(@"https://api.line.me/v2/bot/message/" + linereq.Message.Id + "/content");
//                    m.Headers.Add("Authorization", "Bearer {" + this.ChannelAccessToken  + "}");
//                    req.Media.Add(m);
//                    MessageLogger.InputText += " [media=" + m.Url + "]";
//                }
//                //位置
//                else if (linereq.Message.Type == MessageType.Location)
//                {
//                    var loc = new Location();
//                    loc.Latitude = linereq.Message.Latitude;
//                    loc.Longitude = linereq.Message.Longitude;
//                    req.Location = loc;
//                    MessageLogger.InputText += " [location=" + Json.Encode(loc) + "]";
//                }
//                //スタンプ
//                else if (linereq.Message.Type == MessageType.Sticker)
//                {
//                    req.Sticker = new Sticker(linereq.Message.PackageId, linereq.Message.StickerId);
//                }

//                var userId = linereq.Source.UserId;
//                req.User = UserManager.GetUser(userId);

//                MessageLogger.UserId = req.User.Id;
//                MessageLogger.UserName = req.User.Name;
//                MessageLogger.EventType = linereq.Type;
//                MessageLogger.MessageType = linereq.Message.Type;
//                MessageLogger.InputText = linereq.Message.Text != null ? linereq.Message.Text : "";

//                //BOTロジックの実行
//                var res = MinetteCore.Execute(req);

//                //ユーザの保存
//                UserManager.SaveUser(req.User);

//                //レスポンスのセットアップ
//                var lineres = new LineResponse(linereq.ReplyToken);
//                //スタンプ
//                if(res.Type == ResponseType.Sticker)
//                {
//                    lineres.addStickerMessage(res.Sticker.PackageId, res.Sticker.StickerId);
//                }
//                //画像

//                //テンプレート
//                else if(res.Type == ResponseType.Template)
//                {
//                    if(res.Templates[0].Type == TemplateType.Confirm)
//                    {
//                        lineres.addConfirmMessage(res);
//                    }
//                    else if(res.Templates.Count == 1)
//                    {
//                        lineres.addButtonMessage(res);
//                    }
//                    else
//                    {
//                        lineres.addCarouselMessage(res);
//                    }

//                    MinetteCore.Logger.Write("[LINE]res : " + Json.Encode(lineres));
//                }

//                //それ以外
//                else
//                {
//                    lineres.addTextMessage(res.Text);
//                }
//                //レスポンスの送信
//                MessageLogger.OutputText = res.Text;
//                await SendResponseAsync(lineres);
//            }
//            catch (Exception ex)
//            {
//                MinetteCore.Logger.Write("[LINE]req : " + decodedString);
//                MinetteCore.Logger.Write("[LINE]ProcessRequestError : " + ex.Message + ex.StackTrace);
//            }
//            MessageLogger.Write();
//        }

//        //レスポンスの送信
//        private async Task SendResponseAsync(LineResponse res)
//        {
//            var req = System.Net.WebRequest.Create("https://api.line.me/v2/bot/message/reply");
//            req.Headers.Add("Authorization", "Bearer {" + ChannelAccessToken + "}");
//            req.Method = "POST";
//            req.ContentType = "application/json; charset=UTF-8";
//            var reqJson = Json.Encode(res);
//            byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(reqJson);
//            req.ContentLength = postDataBytes.Length;
//            var reqStream = req.GetRequestStream();
//            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
//            reqStream.Close();
//            var webres = await req.GetResponseAsync();
//            var resStream = webres.GetResponseStream();
//            var sr = new System.IO.StreamReader(resStream, System.Text.Encoding.UTF8);
//            var resFromLine = sr.ReadToEnd();
//            sr.Close();
//        }
//    }

//    //LINEリクエスト
//    public class LineRequest
//    {
//        public List<WebhookEvent> Events { get; set; }
//        public LineRequest()
//        {
//            this.Events = new List<WebhookEvent>();
//        }
//        public LineRequest(dynamic json)
//        {
//            this.Events = new List<WebhookEvent>();
//            if (json.events != null)
//            {
//                foreach (var ev in json.events)
//                {
//                    this.Events.Add(new WebhookEvent(ev));
//                }
//            }
//            else
//            {
//                this.Events.Add(new WebhookEvent(json));
//            }
//        }
//    }
//    public class WebhookEvent
//    {
//        public EventType Type { get; set; }
//        public string ReplyToken { get; set; }
//        public DateTime Timestamp { get; set; }
//        public Source Source { get; set; }
//        public Message Message { get; set; }
//        public Postback Postback { get; set; }
//        public Beacon Beacon { get; set; }
//        public WebhookEvent(dynamic json)
//        {
//            this.Timestamp = new DateTime((long)json.timestamp);
//            this.Source = new Source(json.source);
//            if (json.type == "message")
//            {
//                this.Type = EventType.Message;
//                this.ReplyToken = json.replyToken;
//                this.Message = new Message(json.message);
//            }
//            else if (json.type == "follow")
//            {
//                this.Type = EventType.Follow;
//                this.ReplyToken = json.replyToken;
//            }
//            else if (json.type == "unfollow")
//            {
//                this.Type = EventType.Unfollow;
//            }
//            else if (json.type == "join")
//            {
//                this.Type = EventType.Join;
//                this.ReplyToken = json.replyToken;
//            }
//            else if (json.type == "leave")
//            {
//                this.Type = EventType.Leave;
//            }
//            else if (json.type == "postback")
//            {
//                this.Type = EventType.Postback;
//                this.ReplyToken = json.replyToken;
//            }
//            else if (json.type == "beacon")
//            {
//                this.Type = EventType.Beacon;
//                this.ReplyToken = json.replyToken;
//            }
//        }
//    }
//    public enum SourceType { User = 1, Group, Room };
//    public class Source
//    {
//        public SourceType Type { get; set; }
//        public string UserId { get; set; }
//        public string GroupId { get; set; }
//        public string RoomId { get; set; }
//        public Source(dynamic json)
//        {
//            if (json.type == "user")
//            {
//                this.Type = SourceType.User;
//                this.UserId = json.userId;
//            }
//            else if (json.type == "group")
//            {
//                this.Type = SourceType.Group;
//                this.UserId = json.groupId;
//            }
//            else if (json.type == "room")
//            {
//                this.Type = SourceType.Room;
//                this.UserId = json.roomId;
//            }
//        }

//    }
//    public class Message
//    {
//        public MessageType Type { get; set; }
//        public string Id { get; set; }
//        public string Text { get; set; }
//        public string Title { get; set; }
//        public string Address { get; set; }
//        public decimal Latitude { get; set; }
//        public decimal Longitude { get; set; }
//        public string PackageId { get; set; }
//        public string StickerId { get; set; }
//        public byte[] BinaryData { get; set; }
//        public Message(dynamic json)
//        {
//            if (json.type == "text")
//            {
//                this.Type = MessageType.Text;
//                this.Id = json.id;
//                this.Text = json.text;
//            }
//            else if (json.type == "image")
//            {
//                this.Type = MessageType.Image;
//                this.Id = json.id;
//                //LoadContent(this.Id);
//            }
//            else if (json.type == "video")
//            {
//                this.Type = MessageType.Video;
//                this.Id = json.id;
//                //LoadContent(this.Id);
//            }
//            else if (json.type == "audio")
//            {
//                this.Type = MessageType.Audio;
//                this.Id = json.id;
//                //LoadContent(this.Id);
//            }
//            else if (json.type == "location")
//            {
//                this.Type = MessageType.Location;
//                this.Id = json.id;
//                this.Title = json.title;
//                this.Address = json.address;
//                this.Latitude = json.latitude;
//                this.Longitude = json.longitude;
//            }
//            else if (json.type == "sticker")
//            {
//                this.Type = MessageType.Sticker;
//                this.Id = json.id;
//                this.PackageId = json.packageId;
//                this.StickerId = json.stickerId;
//            }

//        }
//    }
//    public class Postback
//    {
//        public string Data { get; set; }
//        public Postback(dynamic json)
//        {
//            this.Data = json.data;
//        }
//    }
//    public class Beacon
//    {
//        public string Hwid { get; set; }
//        public string Type { get; set; }
//        public Beacon(dynamic json)
//        {
//            this.Hwid = json.hwid;
//            this.Type = "enter";
//        }
//    }
//    //LINEレスポンス
//    public class LineResponse
//    {
//        public string replyToken { get; set; }
//        public List<SendMessage> messages { get; set; }
//        public LineResponse()
//        {
//            this.messages = new List<SendMessage>();
//        }
//        public LineResponse(string replyToken)
//        {
//            this.replyToken = replyToken;
//            this.messages = new List<SendMessage>();
//        }
//        public void addTextMessage(string text)
//        {
//            var msg = new SendMessage();
//            msg.type = "text";
//            msg.text = text;
//            this.messages.Add(msg);
//        }
//        public void addImageMessage(string mediaUrl, string thumbUrl)
//        {
//            var msg = new SendMessage();
//            msg.type = "image";
//            msg.originalContentUrl = mediaUrl;
//            msg.previewImageUrl = thumbUrl;
//            this.messages.Add(msg);
//        }
//        public void addVideoMessage(string mediaUrl, string thumbUrl)
//        {
//            var msg = new SendMessage();
//            msg.type = "video";
//            msg.originalContentUrl = mediaUrl;
//            msg.previewImageUrl = thumbUrl;
//            this.messages.Add(msg);
//        }
//        public void addAudioMessage(string mediaUrl, int duration)
//        {
//            var msg = new SendMessage();
//            msg.type = "audio";
//            msg.originalContentUrl = mediaUrl;
//            msg.duration = duration;
//            this.messages.Add(msg);
//        }
//        public void addLocationMessage(string title, string address, decimal latitude, decimal longitude)
//        {
//            var msg = new SendMessage();
//            msg.type = "location";
//            msg.title = title;
//            msg.address = address;
//            msg.latitude = latitude;
//            msg.longitude = longitude;
//            this.messages.Add(msg);
//        }
//        public void addStickerMessage(string packageId, string stickerId)
//        {
//            var msg = new SendMessage();
//            msg.type = "sticker";
//            msg.packageId = packageId;
//            msg.stickerId = stickerId;
//            this.messages.Add(msg);
//        }
//        public void addImagemapMessage(string altText, string baseUrl, int heightPercentage, List<Action> actions)
//        {
//            var msg = new SendMessage();
//            msg.type = "imagemap";
//            msg.altText = altText;
//            msg.baseUrl = baseUrl;
//            msg.baseSize = new BaseSize(1040 * heightPercentage / 100);
//            msg.actions = actions;
//            this.messages.Add(msg);
//        }
//        public void addButtonMessage(string text, string altText, string title, string thumbUrl, List<TemplateAction> actions)
//        {
//            var msg = new SendMessage();
//            msg.type = "template";
//            msg.altText = altText;
//            var template = new ButtonTemplate();
//            template.text = text;
//            template.title = title;
//            template.thumbnailImageUrl = thumbUrl;
//            template.actions = actions;
//            msg.template = template;
//            this.messages.Add(msg);
//        }
//        public void addButtonMessage(Response response)
//        {
//            var actions = new List<TemplateAction>();
//            foreach(var b in response.Templates[0].Buttons)
//            {
//                if (b.Type == ButtonType.Postback)
//                {
//                    actions.Add(new TemplateAction()
//                    {
//                        type = "postback",
//                        label = b.Title,
//                        data = b.Payload
//                    });
//                }
//                else
//                {
//                    actions.Add(new TemplateAction()
//                    {
//                        type = "uri",
//                        label = b.Title,
//                        uri = b.Url
//                    });
//                }
//            }
//            addButtonMessage(response.Templates[0].Text, response.Templates[0].Text, response.Templates[0].Title, null, actions);
//        }
//        public void addConfirmMessage(string text, string altText, string yesLabel, string yesData, string noLabel, string noData)
//        {
//            var msg = new SendMessage();
//            msg.type = "template";
//            msg.altText = altText;
//            var template = new ConfirmTemplate();
//            template.text = text;
//            template.actions.Add(new Action() { type = "postback", data = yesData, label = yesLabel });
//            template.actions.Add(new Action() { type = "postback", data = noData, label = noLabel });
//            msg.template = template;
//            this.messages.Add(msg);
//        }
//        public void addConfirmMessage(Response response)
//        {
//            addConfirmMessage(
//            response.Templates[0].Text, response.Templates[0].Text,
//                response.Templates[0].Buttons[0].Title,
//                response.Templates[0].Buttons[0].Payload,
//                response.Templates[0].Buttons[1].Title,
//                response.Templates[0].Buttons[1].Payload
//            );
//        }

//        public void addCarouselMessage(string altText, List<Column> columns)
//        {
//            var msg = new SendMessage();
//            msg.type = "template";
//            msg.altText = altText;
//            var template = new CarouselTemplate();
//            template.columns = columns;
//            msg.template = template;
//            this.messages.Add(msg);
//        }
//        public void addCarouselMessage(Response response)
//        {
//            var columns = new List<Column>();
//            foreach(var t in response.Templates)
//            {
//                var actions = new List<TemplateAction>();
//                foreach (var b in t.Buttons)
//                {
//                    if (b.Type == ButtonType.Postback)
//                    {
//                        actions.Add(new TemplateAction()
//                        {
//                            type = "postback",
//                            label = b.Title,
//                            data = b.Payload
//                        });
//                    }
//                    else
//                    {
//                        actions.Add(new TemplateAction()
//                        {
//                            type = "uri",
//                            label = b.Title,
//                            uri = b.Url
//                        });
//                    }
//                }
//                var c = new Column();
//                c.actions = actions;
//                c.title = t.Title;
//                c.text = t.Text;
//                columns.Add(c);
//            }
//            addCarouselMessage(response.Templates[0].Text, columns);
//        }

//    }
//    public class SendMessage
//    {
//        public string type { get; set; }
//        public string text { get; set; }
//        //media
//        public string originalContentUrl { get; set; }
//        public string previewImageUrl { get; set; }
//        //audio
//        public int duration { get; set; }
//        //location
//        public string title { get; set; }
//        public string address { get; set; }
//        public decimal latitude { get; set; }
//        public decimal longitude { get; set; }
//        //sticker
//        public string packageId { get; set; }
//        public string stickerId { get; set; }
//        //rich(imagemap & template)
//        public string baseUrl { get; set; }
//        public string altText { get; set; }
//        public BaseSize baseSize { get; set; }
//        public List<Action> actions { get; set; }
//        public Template template { get; set; }
//    }
//    public class BaseSize
//    {
//        public int height { get; set; }
//        public int width { get; set; }
//        public BaseSize()
//        {
//            this.height = 1040;
//            this.width = 1040;
//        }
//        public BaseSize(int height)
//        {
//            this.height = height;
//            this.width = 1040;
//        }
//    }
//    public class Action
//    {
//        public string type { get; set; }
//        public string label { get; set; }
//        public string text { get; set; }
//        public string data { get; set; }
//        public string linkUri { get; set; }
//        public ImageapArea area { get; set; }
//        public Action() { }
//        public Action(string type)
//        {
//            this.type = type;
//        }
//        public Action(string linkUri, int x, int y, int width, int height)
//        {
//            this.type = "uri";
//            this.linkUri = linkUri;
//            this.area = new ImageapArea();
//            this.area.x = x;
//            this.area.y = y;
//            this.area.width = width;
//            this.area.height = height;
//        }
//    }
//    public class ImageapArea
//    {
//        public int x { get; set; }
//        public int y { get; set; }
//        public int width { get; set; }
//        public int height { get; set; }
//        public ImageapArea() { }
//        public ImageapArea(int x, int y, int width, int height)
//        {
//            this.x = x;
//            this.y = y;
//            this.width = width;
//            this.height = height;
//        }
//    }
//    public class Template
//    {
//        public string type { get; set; }
//    }
//    public class ButtonTemplate : Template
//    {
//        public string thumbnailImageUrl { get; set; }
//        public string title { get; set; }
//        public string text { get; set; }
//        public List<TemplateAction> actions { get; set; }
//        public ButtonTemplate()
//        {
//            this.type = "buttons";
//            this.actions = new List<TemplateAction>();
//        }
//    }
//    public class ConfirmTemplate : Template
//    {
//        public string text { get; set; }
//        public List<Action> actions { get; set; }
//        public ConfirmTemplate()
//        {
//            this.type = "confirm";
//            this.actions = new List<Action>();
//        }
//    }
//    public class CarouselTemplate : Template
//    {
//        public List<Column> columns { get; set; }
//        public CarouselTemplate()
//        {
//            this.type = "carousel";
//            this.columns = new List<Column>();
//        }
//    }
//    public class Column
//    {
//        public string thumbnailImageUrl { get; set; }
//        public string title { get; set; }
//        public string text { get; set; }
//        public List<TemplateAction> actions { get; set; }
//        public Column()
//        {
//            this.actions = new List<TemplateAction>();
//        }
//    }
//    public class TemplateAction
//    {
//        public string type { get; set; }
//        public string label { get; set; }
//        public string text { get; set; }
//        public string data { get; set; }
//        public string uri { get; set; }
//        public TemplateAction() { }
//        public TemplateAction(string label, string data)
//        {
//            this.type = "postback";
//            this.label = label;
//            this.data = data;
//        }
//    }
//}
