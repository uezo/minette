using Minette.Message;

namespace Minette.Application
{
    /// <summary>
    /// Base class of DialogService
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Request from user
        /// </summary>
        public Request Request { get; set; }
        /// <summary>
        /// Data store of successive conversation
        /// </summary>
        public Session Session { get; set; }
        /// <summary>
        /// Global logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Process the request data and setup session data for response
        /// </summary>
        public virtual void ProcessRequest()
        {
            return;
        }

        /// <summary>
        /// Compose response message by using session data
        /// </summary>
        /// <returns>Response to be returned to the user</returns>
        public virtual Response ComposeResponse()
        {
            var ret = new Response(Request.MessageId, ResponseType.Text);
            ret.Text = Session.RestoreData("You said: " + Request.Text);
            return ret;
        }
    }
}
