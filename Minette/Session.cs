using Minette.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Minette
{
    public enum ModeStatus { Start = 1, Continue, End };
    //セッションのデータ型
    public class Session
    {
        public string Id { get; set; }
        public bool IsNew { get; set; }
        public User User { get; set; }
        public string Mode { get; set; }
        public ModeStatus ModeStatus { get; set; }
        public bool KeepMode { get; set; }
        public string PreviousMode { get; set; }
        public string DialogStatus { get; set; }
        public string ChatContext { get; set; }
        public dynamic Data { get; set; }

        [JsonIgnore]
        public IDialogService DialogService { get; set; }

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
            return this.Data;
        }
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
                this.Data = newTDelagate;
            }
            return this.Data;
        }
        public delegate T NewTDelegate<T>();
    }
}
