using Minette.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Minette
{
    public enum ModeStatus { Start = 1, Continue, End };
    /// <summary>
    /// Data container across the requests
    /// </summary>
    public class Session
    {
        /// <summary>
        /// The key for SessionManager
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Indicates that the session was just created in the currently processing request
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// User. Updates to Data, Name and NickName property are saved by UserManager
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Get or Set the topic of current conversasion. Useful in classifying the request in continuing conversasion
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// Indicates the Mode is continuing or just started
        /// </summary>
        public ModeStatus ModeStatus { get; set; }
        /// <summary>
        /// Set true if you want to use the Mode and Data in the next request. Default is false
        /// </summary>
        public bool KeepMode { get; set; }
        public string PreviousMode { get; set; }
        /// <summary>
        /// Get or Set the topic of current status of specific dialog. Useful in processing the request and composing the response message
        /// </summary>
        public string DialogStatus { get; set; }
        /// <summary>
        /// The context Id of docomo Chat API
        /// </summary>
        public string ChatContext { get; set; }
        /// <summary>
        /// Get or Set the data used in DialogServices
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// Set a DialogService in Classifier#Classify() to process the request
        /// </summary>
        [JsonIgnore]
        public IDialogService DialogService { get; set; }

        /// <summary>
        /// New Session with Session ID
        /// </summary>
        /// <param name="Id">Session ID</param>
        public Session(string Id)
        {
            this.Id = Id;
            this.IsNew = true;
            this.Mode = "";
            this.ModeStatus = ModeStatus.Start;
            this.KeepMode = false;
            this.PreviousMode = "";
            this.DialogStatus = "";
            this.ChatContext = "";
            this.DialogService = new Minette.Application.DialogService();
        }

        /// <summary>
        /// Restored Session from default database
        /// </summary>
        /// <param name="r">A record from session table</param>
        public Session(dynamic r)
        {
            this.Id = (string)r["Id"];
            this.IsNew = false;
            this.Mode = (string)r["Mode"];
            this.ModeStatus = this.Mode != "" ? ModeStatus.Continue : ModeStatus.Start;
            this.KeepMode = false;
            this.PreviousMode = "";
            this.DialogStatus = (string)r["DialogStatus"];
            this.ChatContext = (string)r["ChatContext"];
            var d = (string)r["Data"];
            this.Data = Json.Decode(d);
            this.DialogService = new Minette.Application.DialogService();
        }

        /// <summary>
        /// Restore or create the type specified data
        /// </summary>
        /// <returns>Default value for return when the Data properties is empty</returns>
        /// <returns>Reference to Data property</returns>
        public virtual T RestoreData<T>(T defaultValue)
        {
            if (this.Data is T)
            {
                //何もしない
            }
            else if (this.Data is JObject)
            {
                this.Data = ((JObject)this.Data).ToObject<T>();
            }
            else
            {
                this.Data = defaultValue;
            }
            return (T)this.Data;
        }

        /// <summary>
        /// Restore or create the type specified data
        /// </summary>
        /// <returns>Delegate to create the default value for return when the Data properties is empty</returns>
        /// <returns>Reference to Data property</returns>
        public virtual T RestoreData<T>(NewTDelegate<T> newTDelagate)
        {
            if (this.Data is T)
            {
                //何もしない
            }
            else if (this.Data is JObject)
            {
                //JSONからデシリアライズしたものは型変換して再格納
                this.Data = ((JObject)this.Data).ToObject<T>();
            }
            else
            {
                //データが未格納の場合は生成処理を行って格納
                this.Data = newTDelagate();
            }
            return (T)this.Data;
        }
        public delegate T NewTDelegate<T>();
    }
}
