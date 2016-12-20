using Minette.Message;

namespace Minette.Application
{
    public class ChatDialog : IDialogService
    {
        public Request Request { get; set; }
        public Session Session { get; set; }
        public ILogger Logger { get; set; }
        public string ApiKey { get; set; }

        public ChatDialog(string apiKey) {
            this.ApiKey = apiKey;
        }

        public void ProcessRequest()
        {
            var d = new Minette.WebService.Docomo.Dialogue(ApiKey, Logger);
            Session.Data = d.Chat(Request.Text, Session).Replace("26歳", "16歳");
            if (Session.Mode == "srtr")
            {
                Session.KeepMode = true;
            }
        }

        public Response ComposeResponse()
        {
            var ret = new Response(Request.MessageId, ResponseType.Text);
            ret.Text = (string)Session.Data;
            return ret;
        }
    }
}
