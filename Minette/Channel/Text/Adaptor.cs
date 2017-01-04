using System;
using Minette.Message;
using Minette.Application;

namespace Minette.Channel.Text
{
    public class Adaptor
    {
        public Minette.Core MinetteCore { get; set; }
        public IUserManager UserManager { get; set; }
        public IMessageLogger MessageLogger { get; set; }

        public Adaptor(){
            this.MinetteCore = new Minette.Core();
            this.UserManager = new UserManager();
            this.MessageLogger = new MessageLogger("Text");
        }
        public Adaptor(Minette.Core minetteCore, IUserManager userManager, IMessageLogger messageLogger)
        {
            this.MinetteCore = minetteCore;
            this.UserManager = userManager;
            this.MessageLogger = messageLogger;
        }

        public string ProcessRequest(string inputMessage, string userId)
        {
            var ret = "";
            try
            {
                //リクエストのセットアップ
                var req = new Request(userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"), "Text");
                req.IsPrivate = true;
                req.Text = inputMessage;
                req.User = UserManager.GetUser(userId);
                req.User.ChannelId = userId;
                MessageLogger.InputText = req.Text;
                MessageLogger.UserId = req.User.Id;
                MessageLogger.UserName = req.User.Name;

                //BOTロジックの実行
                var res = MinetteCore.Execute(req);

                //ユーザの保存
                UserManager.SaveUser(req.User);

                //レスポンスの返却
                MessageLogger.OutputText = res.Text;
                ret = res.Text;
            }
            catch (Exception ex)
            {
                MinetteCore.Logger.Write("[Text]ProcessRequestError : " + ex.Message + "\n" + ex.StackTrace);
            }
            MessageLogger.Write();
            return ret;
        }
    }
}
