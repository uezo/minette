using System;
using Minette.Util;
using Minette.Message;
using Minette.Application;

namespace Minette
{
    //フレームワークコア機能
    public class Core
    {
        public IClassifier Classifier { get; set; }
        public ISessionManager SessionManager { get; set; }
        public ILogger Logger { get; set; }
        public ITagger Tagger { get; set; }
        public bool Debug { get; set; }

        //コンストラクタ
        public Core()
        {
            this.Classifier = new Classifier();
            this.SessionManager = new SessionManager();
            this.Logger = new Logger();
            this.Tagger = new Tagger();
            this.Debug = false;
        }

        /// <summary>
        /// Execute bot program
        /// </summary>
        /// <param name="request">Request converted from channel request</param>
        public Response Execute(Request request)
        {
            var res = new Response(request.MessageId, ResponseType.Text);
            res.Text = "？";
            request.Words = Tagger.Parse(request.Text);
            var session = SessionManager.GetSession(request.Channel + "::" + request.User.ChannelId);
            session.User = request.User;
            var LogLines = "";
            try
            {
                LogLines += "[minette]Start : " + Json.Encode(session) + "\n";
                //話題の判定
                Classifier.Request = request;
                Classifier.Session = session;
                Classifier.Logger = Logger;
                Classifier.Classify();
                LogLines += "[minette]Mode=" + session.Mode + " : " + Json.Encode(session) + "\n";
                //ダイアログの準備
                session.DialogService.Request = request;
                session.DialogService.Session = session;
                session.DialogService.Logger = Logger;
                //処理の実行
                session.DialogService.ProcessRequest();
                //応答メッセージの取得
                res = session.DialogService.ComposeResponse();
                LogLines += "[minette]End : " + Json.Encode(session) + "\n";
            }
            catch (Exception ex)
            {
                Logger.Write(LogLines);
                Logger.Write("[minette]Error : " + ex.Message + ex.StackTrace);
                session.KeepMode = false;
            }
            if (Debug)
            {
                Logger.Write(LogLines);
            }
            if (!session.KeepMode)
            {
                session.Mode = "";
                session.DialogStatus = "";
                session.Data = null;
            }
            SessionManager.SaveSession(session);
            return res;
        }
    }

    //形態素データ型
    public class MecabNode
    {
        public string Word { get; set; }
        public string Part { get; set; }
        public string PartDetail1 { get; set; }
        public string PartDetail2 { get; set; }
        public string PartDetail3 { get; set; }
        public string StemType { get; set; }
        public string StemForm { get; set; }
        public string OriginalFormWord { get; set; }
        public string Kana { get; set; }
        public string Pronunciation { get; set; }
        public MecabNode() { }
        public MecabNode(dynamic node)
        {
            this.Word = node.Word;
            this.Part = node.Part;
            this.PartDetail1 = node.PartDetail1;
            this.PartDetail2 = node.PartDetail2;
            this.PartDetail3 = node.PartDetail3;
            this.StemType = node.StemType;
            this.StemForm = node.StemForm;
            this.OriginalFormWord = node.OriginalFormWord;
            this.Kana = node.Kana;
            this.Pronunciation = node.Pronunciation;
        }
    }
}
