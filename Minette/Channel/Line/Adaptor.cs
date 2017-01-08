using System;
using System.Threading.Tasks;
using System.Threading;
using Minette.Util;
using Minette.Message;
using Minette.Message.Attachment;
using Minette.Application;

namespace Minette.Channel.Line
{
    public class Adaptor
    {
        public Minette.Core MinetteCore { get; set; }
        public IUserManager UserManager { get; set; }
        public IMessageLogger MessageLogger { get; set; }
        public string ChannelAccessToken { get; set; }

        public Adaptor()
        {
            this.MinetteCore = new Minette.Core();
            this.UserManager = new UserManager();
            this.MessageLogger = new MessageLogger("LINE");
        }
        public Adaptor(Minette.Core minetteCore, IUserManager userManager, IMessageLogger messageLogger, string channelAccessToken)
        {
            this.MinetteCore = minetteCore;
            this.UserManager = userManager;
            this.MessageLogger = messageLogger;
            this.ChannelAccessToken = channelAccessToken;
        }

        public async Task ProcessRequestAsync(CancellationToken cancellationToken, string decodedString)
        {
            try
            {
                //リクエストのセットアップ
                var linereq = new Request((Json.Decode(decodedString)).events[0]).Events[0];
                var req = new Minette.Message.Request(linereq.Message.Id, "LINE");
                //テキスト
                if (linereq.Message.Type == MessageType.Text)
                {
                    req.Text = linereq.Message.Text;
                }
                //画像
                else if (linereq.Message.Type == MessageType.Image)
                {
                    var m = new Media(@"https://api.line.me/v2/bot/message/" + linereq.Message.Id + "/content");
                    m.Headers.Add("Authorization", "Bearer {" + this.ChannelAccessToken + "}");
                    req.Media.Add(m);
                    MessageLogger.InputText += " [media=" + m.Url + "]";
                }
                //位置
                else if (linereq.Message.Type == MessageType.Location)
                {
                    var loc = new Location();
                    loc.Latitude = linereq.Message.Latitude;
                    loc.Longitude = linereq.Message.Longitude;
                    req.Location = loc;
                    MessageLogger.InputText += " [location=" + Json.Encode(loc) + "]";
                }
                //スタンプ
                else if (linereq.Message.Type == MessageType.Sticker)
                {
                    req.Sticker = new Sticker(linereq.Message.PackageId, linereq.Message.StickerId);
                }

                var userId = linereq.Source.UserId;
                req.User = UserManager.GetUser(userId);

                MessageLogger.UserId = req.User.Id;
                MessageLogger.UserName = req.User.Name;
                MessageLogger.EventType = linereq.Type;
                MessageLogger.MessageType = linereq.Message.Type;
                MessageLogger.InputText = linereq.Message.Text != null ? linereq.Message.Text : "";

                //BOTロジックの実行
                var res = MinetteCore.Execute(req);

                //ユーザの保存
                UserManager.SaveUser(req.User);

                //レスポンスのセットアップ
                var lineres = new Response(linereq.ReplyToken);

                //テキスト
                if (res.Type == ResponseType.Text)
                {
                    lineres.addTextMessage(res.Text);
                    MessageLogger.OutputText = res.Text;
                }
                //スタンプ
                else if (res.Type == ResponseType.Sticker)
                {
                    lineres.AddStickerMessage(res.Sticker.PackageId, res.Sticker.StickerId);
                    MessageLogger.OutputText = "sticker: " + res.Sticker.PackageId + "/" + res.Sticker.StickerId;
                }
                //テンプレート
                else if (res.Type == ResponseType.Template)
                {
                    if (res.Templates[0].Type == TemplateType.Confirm)
                    {
                        lineres.AddConfirmMessage(res);
                    }
                    else if (res.Templates.Count == 1)
                    {
                        lineres.AddButtonMessage(res);
                    }
                    else
                    {
                        lineres.AddCarouselMessage(res);
                    }
                    MessageLogger.OutputText = "templates: " + Json.Encode(res.Templates);
                }
                //それ以外（テキスト・スタンプ・テンプレート以外は現時点で未対応）
                else
                {
                    lineres.addTextMessage(res.Text);
                    MessageLogger.OutputText = res.Text;
                }

                //テンプレートメッセージ以外でテンプレートがあればメッセージを追加
                if (res.Type != ResponseType.Template && res.Templates != null && res.Templates.Count > 0)
                {
                    if (res.Templates[0].Type == TemplateType.Confirm)
                    {
                        lineres.AddConfirmMessage(res);
                    }
                    else if (res.Templates.Count == 1)
                    {
                        lineres.AddButtonMessage(res);
                    }
                    else
                    {
                        lineres.AddCarouselMessage(res);
                    }
                    MessageLogger.OutputText += " / additional templates: " + Json.Encode(res.Templates);
                }

                //レスポンスの送信
                await SendResponseAsync(lineres);
            }
            catch (Exception ex)
            {
                MinetteCore.Logger.Write("[LINE]req : " + decodedString);
                MinetteCore.Logger.Write("[LINE]ProcessRequestError : " + ex.Message + ex.StackTrace);
            }
            MessageLogger.Write();
        }

        //レスポンスの送信
        private async Task SendResponseAsync(Response res)
        {
            var req = System.Net.WebRequest.Create("https://api.line.me/v2/bot/message/reply");
            req.Headers.Add("Authorization", "Bearer {" + ChannelAccessToken + "}");
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            var reqJson = Json.Encode(res);
            if (MinetteCore.Debug == true)
            {
                MinetteCore.Logger.Write("[LINE]res : " + reqJson);
            }
            byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(reqJson);
            req.ContentLength = postDataBytes.Length;
            var reqStream = req.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();
            var webres = await req.GetResponseAsync();
            var resStream = webres.GetResponseStream();
            var sr = new System.IO.StreamReader(resStream, System.Text.Encoding.UTF8);
            var resFromLine = sr.ReadToEnd();
            sr.Close();
        }
    }
}
