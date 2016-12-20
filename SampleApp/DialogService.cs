using Minette;
using Minette.Message;

namespace SampleApp
{
    public class DialogService : Minette.Application.DialogService
    {
        //リクエスト処理部。Session.Dataのセットアップに特化し、応答メッセージを意識しない
        public override void ProcessRequest()
        {
            //入力した数値を3倍にしてセッションデータに格納（文字だとエラー）
            var inputNumber = int.Parse(Request.Text);
            Session.Data = inputNumber * 3;
        }

        //応答メッセージ作成部。Session.Dataの値からのメッセージ作成に特化し、なるべく処理は行わない
        public override Response ComposeResponse()
        {
            //レスポンスの初期化
            var res = new Response(Request.MessageId, ResponseType.Text);
            //セッションデータを使ってメッセージを作成
            res.Text = "入力値を3倍すると" + ((int)Session.Data).ToString() + "です";
            return res;
        }
    }
}
