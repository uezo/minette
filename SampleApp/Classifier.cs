using Minette;
using Minette.Message;

namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        private const string DocomoAPIKey = "<input your docomo api key>";

        public override void Classify()
        {
            Session.Data = "Hello, Minette!";

            ////STEP2 入力に応じた応答
            //if (Request.Text == "おはよう")
            //{
            //    Session.Data = "おはようございます";
            //}
            //else
            //{
            //    Session.Data = "こんにちは";
            //}

            ////STEP3 雑談
            //Session.DialogService = new Minette.Application.ChatDialog(DocomoAPIKey);

            ////STEP4 自作のダイアログを利用
            //Session.DialogService = new DialogService();

            ////STEP5 入力に応じてダイアログを切り替える
            //if (Request.Text.Contains("1"))
            //{
            //    //数値を含む場合は自作ダイアログ
            //    Session.DialogService = new DialogService();
            //}
            //else
            //{
            //    //それ以外の場合は雑談
            //    Session.DialogService = new Minette.Application.ChatDialog(DocomoAPIKey);
            //}
        }
    }
}
