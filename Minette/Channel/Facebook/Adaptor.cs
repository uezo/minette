using System;
using System.Threading.Tasks;
using System.Threading;
using Minette.Util;
using Minette.Message;
using Minette.Message.Attachment;
using Minette.Application;

namespace Minette.Channel.Facebook
{
    public class Adaptor
    {
        public Minette.Core MinetteCore { get; set; }
        public IUserManager UserManager { get; set; }
        public IMessageLogger MessageLogger { get; set; }
        public string AccessToken { get; set; }

        public Adaptor()
        {
            this.MinetteCore = new Minette.Core();
            this.UserManager = new UserManager();
            this.MessageLogger = new MessageLogger("Facebook");
        }
        public Adaptor(Minette.Core minetteCore, IUserManager userManager, IMessageLogger messageLogger, string accessToken)
        {
            this.MinetteCore = minetteCore;
            this.UserManager = userManager;
            this.MessageLogger = messageLogger;
            this.AccessToken = accessToken;
        }

        public async Task ProcessRequestAsync(CancellationToken cancellationToken, string decodedString)
        {
            try
            {
                //リクエストのセットアップ
                var jsonEntry = Json.Decode(decodedString);
                var json = jsonEntry.entry[0].messaging[0];

                //通常のメッセージとpostbackとでリクエストのセットアップが異なる
                var req = new Request("", "Facebook");
                if (json.message != null)
                {
                    req.MessageId = (string)json.message.mid;

                    //テキストメッセージ
                    if (json.message.text != null)
                    {
                        req.Text = json.message.text;
                        MessageLogger.InputText = req.Text;
                    }
                    //画像
                    if (json.message.attachments != null && json.message.attachments[0].type == "image")
                    {
                        var m = new Media((string)json.message.attachments[0].payload.url);
                        req.Media.Add(m);
                        MessageLogger.InputText += " [media=" + m.Url + "]";
                    }
                    //位置
                    if (json.message.attachments != null && json.message.attachments[0].type == "location")
                    {
                        var loc = new Location();
                        loc.Latitude = json.message.attachments[0].payload.coordinates.lat;
                        loc.Longitude = json.message.attachments[0].payload.coordinates["long"];
                        req.Location = loc;
                        MessageLogger.InputText += " [location=" + Json.Encode(loc) + "]";
                    }
                }
                //postback
                else
                {
                    //payloadをテキストとして扱う
                    req.Text = (string)json.postback.payload;
                    MessageLogger.InputText = req.Text;
                }
                
                //ユーザー情報
                var userId = (string)json.sender.id;
                req.User = UserManager.GetUser(userId);

                MessageLogger.UserId = req.User.Id;
                MessageLogger.UserName = req.User.Name;

                //BOTロジックの実行
                var res = MinetteCore.Execute(req);

                //ユーザの保存
                UserManager.SaveUser(req.User);

                //レスポンスのセットアップ
                var fbres = new Response(userId);

                //テキスト
                if(res.Type == ResponseType.Text)
                {
                    fbres.SetTextMessage(res.Text);
                    MessageLogger.OutputText = res.Text;
                }
                //テンプレート
                else if (res.Type == ResponseType.Template)
                {
                    if (res.Templates[0].Type == Minette.Message.TemplateType.Confirm)
                    {
                        fbres.SetButtonMessage(res.Templates);
                    }
                    else if (res.Templates[0].Type == Minette.Message.TemplateType.Button)
                    {
                        fbres.SetButtonMessage(res.Templates);
                    }
                    else
                    {
                        fbres.SetListMessage(res);
                    }
                    MessageLogger.OutputText = "templates: " + Json.Encode(res.Templates);
                }
                //クイックリプライ
                else if (res.Type == ResponseType.QuickReply)
                {
                    fbres.SetQuickReplyMessage(res.Text, res.QuickReplies);
                    MessageLogger.OutputText = res.Text + " / quick: " + Json.Encode(res.QuickReplies);
                }
                //except Text, Template and QuickReply is not supported now.
                else
                {
                    fbres.SetTextMessage(res.Text);
                    MessageLogger.OutputText = res.Text;
                }

                await SendResponseAsync(fbres);

                //テンプレートメッセージ以外でテンプレートがあればメッセージを追加
                if (res.Type != ResponseType.Template && res.Templates != null && res.Templates.Count > 0)
                {
                    fbres = new Response(userId);
                    if (res.Templates[0].Type == Minette.Message.TemplateType.Confirm)
                    {
                        fbres.SetButtonMessage(res.Templates);
                    }
                    else if (res.Templates[0].Type == Minette.Message.TemplateType.Button)
                    {
                        fbres.SetButtonMessage(res.Templates);
                    }
                    else
                    {
                        fbres.SetListMessage(res);
                    }
                    MessageLogger.OutputText += " / additional templates: " + Json.Encode(res.Templates);
                    await SendResponseAsync(fbres);
                }
            }
            catch (Exception ex)
            {
                MinetteCore.Logger.Write("[Facebook]req : " + decodedString);
                MinetteCore.Logger.Write("[Facebook]ProcessRequestError : " + ex.Message + ex.StackTrace);
            }
            MessageLogger.Write();
        }
        //レスポンスの送信
        private async Task SendResponseAsync(Response res)
        {
            var req = System.Net.WebRequest.Create("https://graph.facebook.com/v2.6/me/messages?access_token=" + this.AccessToken);
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            var reqJson = Json.Encode(res, JsonCase.Snake);
            if(MinetteCore.Debug == true)
            {
                MinetteCore.Logger.Write("[Facebook]res : " + reqJson);
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
