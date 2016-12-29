using System;
using Minette.Message;

namespace Minette.Application
{
    /// <summary>
    /// Base class of Classifier
    /// </summary>
    public class Classifier : IClassifier
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
        /// Classify the mode and setup a proper DialogService
        /// </summary>
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
