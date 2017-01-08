using Minette.Message;
using System.Collections.Generic;

namespace Minette.Application
{
    /// <summary>
    /// DialogService for chatting with docomo API
    /// </summary>
    public class ChatDialog : DialogService
    {
        /// <summary>
        /// docomo API Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Values to replace
        /// </summary>
        public Dictionary<string, string> ReplaceValues { get; set; }

        /// <summary>
        /// Initialize with API Key
        /// </summary>
        public ChatDialog(string apiKey) {
            this.ApiKey = apiKey;
            this.ReplaceValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initialize with API Key and Replace values
        /// </summary>
        public ChatDialog(string apiKey, Dictionary<string, string> replaceValues)
        {
            this.ApiKey = apiKey;
            this.ReplaceValues = replaceValues;
        }

        public override void ProcessRequest()
        {
            var d = new Minette.WebService.Docomo.Dialogue(ApiKey, Logger);
            var chatString = d.Chat(Request.Text, Session);
            foreach(var kv in ReplaceValues)
            {
                chatString = chatString.Replace(kv.Key, kv.Value);
            }
            Session.Data = chatString;
            if (Session.Mode == "srtr")
            {
                Session.KeepMode = true;
            }
        }

        public override Response ComposeResponse()
        {
            var ret = new Response(Request.MessageId, ResponseType.Text);
            ret.Text = (string)Session.Data;
            return ret;
        }
    }
}
