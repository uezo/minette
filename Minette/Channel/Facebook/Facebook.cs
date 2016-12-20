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
//    public class Facebook
//    {
//        public Minette.Core MinetteCore { get; set; }
//        public IUserManager UserManager { get; set; }
//        public IMessageLogger MessageLogger { get; set; }
//        public string AccessToken { get; set; }

//        public Facebook()
//        {
//            this.MinetteCore = new Minette.Core();
//            this.UserManager = new UserManager();
//            this.MessageLogger = new MessageLogger("Facebook");
//        }
//        public Facebook(Minette.Core minetteCore, IUserManager userManager, IMessageLogger messageLogger, string accessToken)
//        {
//            this.MinetteCore = minetteCore;
//            this.UserManager = userManager;
//            this.MessageLogger = messageLogger;
//            this.AccessToken = accessToken;
//        }

//        public async Task ProcessRequestAsync(CancellationToken cancellationToken, string decodedString)
//        {
//            try
//            {
//                //リクエストのセットアップ
//                var jsonEntry = Json.Decode(decodedString);
//                var json = jsonEntry.entry[0].messaging[0];
//                var req = new Request((string)json.message.mid, "Facebook");

//                //テキストメッセージ
//                if (json.message.text != null)
//                {
//                    req.Text = json.message.text;
//                    MessageLogger.InputText = req.Text;
//                }
//                //画像
//                if(json.message.attachments != null && json.message.attachments[0].type == "image")
//                {
//                    var m = new Media((string)json.message.attachments[0].payload.url);
//                    req.Media.Add(m);
//                    MessageLogger.InputText += " [media=" + m.Url + "]";
//                }
//                //位置
//                if (json.message.attachments != null && json.message.attachments[0].type == "location")
//                {
//                    var loc = new Location();
//                    loc.Latitude = json.message.attachments[0].payload.coordinates.lat;
//                    loc.Longitude = json.message.attachments[0].payload.coordinates["long"];
//                    req.Location = loc;
//                    MessageLogger.InputText += " [location=" + Json.Encode(loc) + "]";
//                }
//                //ユーザー情報
//                var userId = (string)json.sender.id;
//                req.User = UserManager.GetUser(userId);

//                MessageLogger.UserId = req.User.Id;
//                MessageLogger.UserName = req.User.Name;

//                //BOTロジックの実行
//                var res = MinetteCore.Execute(req);

//                //ユーザの保存
//                UserManager.SaveUser(req.User);

//                //レスポンスの返却
//                var fbres = new FacebookResponse();
//                fbres.Message = new TextMessage() { Text = res.Text };
//                fbres.Recipient = new Recipient() { Id = userId };

//                if(res.Type == ResponseType.Template)
//                {
//                    fbres = new FacebookResponse();
//                    fbres.Recipient = new Recipient() { Id = userId };
//                    fbres.SetTemplateMessage(res);
//                }

//                if (res.Type == ResponseType.QuickReply)
//                {
//                    fbres = new FacebookResponse();
//                    var m = new QuickReplyMessage();
//                    m.Text = res.Text;
//                    m.QuickReplies = res.QuickReplies;
//                    fbres.Message = m;
//                    fbres.Recipient = new Recipient() { Id = userId };
//                    MinetteCore.Logger.Write("[Facebook]res : " + Json.Encode(fbres));
//                }

//                await SendResponseAsync(fbres);
//            }
//            catch (Exception ex)
//            {
//                MinetteCore.Logger.Write("[Facebook]req : " + decodedString);
//                MinetteCore.Logger.Write("[Facebook]ProcessRequestError : " + ex.Message + ex.StackTrace);
//            }
//            MessageLogger.Write();
//        }
//        //レスポンスの送信
//        private async Task SendResponseAsync(FacebookResponse res)
//        {
//            var req = System.Net.WebRequest.Create("https://graph.facebook.com/v2.6/me/messages?access_token=" + this.AccessToken);
//            req.Method = "POST";
//            req.ContentType = "application/json; charset=UTF-8";
//            var reqJson = Json.Encode(res, JsonCase.Snake);
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
//    public class FacebookResponse
//    {
//        public Recipient Recipient { get; set; }
//        public FacebookMessage Message { get; set; }

//        public void SetTemplateMessage(Response response)
//        {
//            var m = new TemplateMessage();
//            var p = m.Attachment.Payload as TemplatePayload;
//            if (response.Templates[0].Type == TemplateType.Confirm)
//            {
//                m.Attachment.Payload = new TemplatePayload(FacebookTemplateType.button);
//                (m.Attachment.Payload as TemplatePayload).Text = response.Templates[0].Text;
//                foreach (var b in response.Templates[0].Buttons)
//                {
//                    if (b.Type == ButtonType.Postback)
//                    {
//                        (m.Attachment.Payload as TemplatePayload).Buttons.Add(new Button(FacebookButtonType.postback, b.Title, b.Payload));
//                    }
//                    else
//                    {
//                        (m.Attachment.Payload as TemplatePayload).Buttons.Add(new Button(FacebookButtonType.web_url, b.Title, b.Url));
//                    }
//                }
//            }
//            else
//            {
//                m.Attachment.Payload = new TemplatePayload(FacebookTemplateType.generic);
//                foreach (var t in response.Templates)
//                {
//                    var elm = new PayloadElement();
//                    elm.Title = t.Title;
//                    elm.Subtitle = t.Text;
//                    elm.Buttons = new List<Button>();
//                    foreach (var b in t.Buttons)
//                    {
//                        if (b.Type == ButtonType.Postback)
//                        {
//                            elm.Buttons.Add(new Button(FacebookButtonType.postback, b.Title, b.Payload));
//                        }
//                        else
//                        {
//                            elm.Buttons.Add(new Button(FacebookButtonType.web_url, b.Title, b.Url));
//                        }
//                    }
//                    (m.Attachment.Payload as TemplatePayload).Elements.Add(elm);
//                }
//            }
//            this.Message = m;
//        }
//    }
//    public class Recipient
//    {
//        public string Id { get; set; }
//    }

//    public class FacebookMessage
//    {
//    }

//    public class TextMessage: FacebookMessage
//    {
//        public string Text { get; set; }
//    }

//    public class TemplateMessage: FacebookMessage
//    {
//        public Attachment Attachment { get; set; }
//        public TemplateMessage()
//        {
//            this.Attachment = new Attachment(AttachmentType.template);
//        }
//    }

//    public class QuickReplyMessage : FacebookMessage
//    {
//        public string Text { get; set; }
//        public Attachment Attachment { get; set; }
//        public List<QuickReply> QuickReplies { get; set; }
//        public QuickReplyMessage()
//        {
//            this.QuickReplies = new List<QuickReply>();
//        }
//    }

//    public enum AttachmentType { template = 1, image, audio, video, file }
//    public class Attachment
//    {
//        public AttachmentType Type { get; set; }
//        public Payload Payload { get; set; }
//        public Attachment()
//        {
//        }
//        public Attachment(AttachmentType type)
//        {
//            this.Type = type;
//            if (type == AttachmentType.template)
//            {
//                this.Payload = new TemplatePayload();
//            }
//        }
//        public Attachment(AttachmentType type, FacebookTemplateType templateType)
//        {
//            this.Type = type;
//            if (type == AttachmentType.template)
//            {
//                this.Payload = new TemplatePayload(templateType);
//            }
//        }
//    }

//    //各Payloadの基底
//    public class Payload
//    {
//    }

//    public enum FacebookTemplateType { button = 1, generic, list }
//    public class TemplatePayload : Payload
//    {
//        public FacebookTemplateType TemplateType { get; set; }
//        public string Title { get; set; }
//        public string Text { get; set; }
//        public string ImageUrl { get; set; }
//        public string ItemUrl { get; set; }
//        public List<Button> Buttons { get; set; }
//        public List<PayloadElement> Elements { get; set; }
//        public TemplatePayload() { }
//        public TemplatePayload(FacebookTemplateType type)
//        {
//            this.TemplateType = type;
//            if(type == FacebookTemplateType.button)
//            {
//                this.Buttons = new List<Button>();
//            }
//            else
//            {
//                this.Elements = new List<PayloadElement>();
//            }
//        }
//    }

//    public class PayloadElement
//    {
//        public string Title { get; set; }
//        public string Subtitle { get; set; }
//        public string ImageUrl { get; set; }
//        public string ItemUrl { get; set; }
//        public List<Button> Buttons { get; set; }
//    }

//    public enum FacebookButtonType { web_url = 1, postback }
//    public class Button
//    {
//        public FacebookButtonType Type { get; set; }
//        public string Title { get; set; }
//        public string Url { get; set; }
//        public string Payload { get; set; }
//        public Button()
//        {
//        }
//        public Button(FacebookButtonType type, string title, string data)
//        {
//            this.Type = type;
//            this.Title = title;
//            if (type == FacebookButtonType.web_url)
//            {
//                this.Url = data;
//            }
//            else if(type == FacebookButtonType.postback)
//            {
//                this.Payload = data;
//            }
//        }
//    }
//}
