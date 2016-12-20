using Minette.Message;

namespace Minette.Application
{
    public class DialogService : IDialogService
    {
        public Request Request { get; set; }
        public Session Session { get; set; }
        public ILogger Logger { get; set; }

        public virtual void ProcessRequest()
        {
            return;
        }

        public virtual Response ComposeResponse()
        {
            var ret = new Response(Request.MessageId, ResponseType.Text);
            ret.Text = Session.RestoreData("You said: " + Request.Text);
            return ret;
        }
    }
}
