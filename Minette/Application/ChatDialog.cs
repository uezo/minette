using Minette.Message;

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
        /// Initialize with API Key
        /// </summary>
        public ChatDialog(string apiKey) {
            this.ApiKey = apiKey;
        }

        public override void ProcessRequest()
        {
            var d = new Minette.WebService.Docomo.Dialogue(ApiKey, Logger);
            Session.Data = d.Chat(Request.Text, Session).Replace("26歳", "16歳");
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
