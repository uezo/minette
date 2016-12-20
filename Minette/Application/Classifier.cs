using Minette.Message;

namespace Minette.Application
{
    public class Classifier : IClassifier
    {
        public Request Request { get; set; }
        public Session Session { get; set; }
        public ILogger Logger { get; set; }
        public virtual void GetClassified()
        {
            return;
        }
    }
}
