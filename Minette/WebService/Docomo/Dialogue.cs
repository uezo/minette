using System;
using System.Collections.Generic;
using Minette.Util;

namespace Minette.WebService.Docomo
{
    public class Dialogue
    {
        public string APIKey { get; set; }
        public ILogger Logger { get; set; }
        public Dialogue(string apikey)
        {
            this.APIKey = apikey;
            this.Logger = new Minette.Application.Logger();
        }
        public Dialogue(string apikey, ILogger logger)
        {
            this.APIKey = apikey;
            this.Logger = logger;
        }
        public string Chat(string text)
        {
            return Chat(text, null);
        }
        public string Chat(string text, Session session)
        {
            var ret = "";
            try
            {
                var req = new Dictionary<string, string>();
                req.Add("utt", text);
                req.Add("context", session != null ? session.ChatContext : "");
                req.Add("mode", session != null && session.Mode == "srtr" ? "srtr" : "");
                if (session.User.NickName != "")
                {
                    req.Add("nickname", session.User.NickName);
                }
                var jsonstring = Http.PostJson("https://api.apigw.smt.docomo.ne.jp/dialogue/v1/dialogue?APIKEY=" + this.APIKey, req);
                var json = Json.Decode(jsonstring);
                if(session != null)
                {
                    session.ChatContext = json.context;
                    session.Mode = json.mode == "srtr" ? "srtr" : "";
                }
                ret = json.utt;
            }
            catch(Exception ex)
            {
                Logger.Write("ChatError:" + ex.Message + "\n" + ex.StackTrace);
            }
            return ret;
        }
    }
}
