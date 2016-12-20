using System;
using Minette;
using Minette.Application;

namespace SampleApp
{
    class Program
    {
        public const string DatabaseName = "<input database defined in Web.config>";

        static void Main(string[] args)
        {
            //アダプタのインスタンス化のみ（おうむ返し） / Just instancing(echo)
            var adaptor = new Minette.Channel.Text.Adaptor();
            ////サンプルアプリの利用
            //adaptor.MinetteCore.Classifier = new SampleClassifier();



            ////個別に設定する場合の例 / Example of setting up.

            ////ミネットコアのインスタンス化 / instancing the Core of Minette
            //var minette = new Minette.Core();
            ////分類器 / Set the Classifier
            //minette.Classifier = new Classifier();
            ////セッション管理 / Set the Session Manager
            //minette.SessionManager = new SessionManager(DatabaseName, "<Session Table>");
            ////デバッグロガー / Set the Logger
            //minette.Logger = new Logger(DatabaseName, "<Log Table>");
            ////形態素分析利用有無 / Set enabled/disabled Japanese Tagger.
            //minette.Tagger.Enabled = true;

            ////コネクタのインスタンス化 / Instancing the Connector
            //var adaptor = new Minette.Connector.Text();
            ////ミネットコア / Set the Core of Minette
            //adaptor.MinetteCore = minette;
            ////ユーザー管理 / Set the User Manager
            //adaptor.UserManager = new UserManager(DatabaseName, "<User Table>", "Text");
            ////メッセージロガー / Set the Message Logger
            //adaptor.MessageLogger = new MessageLogger(DatabaseName, "<MessageLogger Table>", "Text");

            while (true)
            {
                Console.Write("user> ");
                var text = Console.ReadLine();
                Console.WriteLine("minette> " + adaptor.ProcessRequest(text, "12345"));
            }
        }
    }
}
