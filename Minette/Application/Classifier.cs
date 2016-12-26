using System;
using Minette.Message;

namespace Minette.Application
{
    public class Classifier : IClassifier
    {
        public Request Request { get; set; }
        public Session Session { get; set; }
        public ILogger Logger { get; set; }
        public virtual void Classify()
        {
            return;
        }
        [Obsolete("Deprecated. Use Classify() instead.")]
        public virtual void GetClassified()
        {
            Classify();
            return;
        }
    }
}
