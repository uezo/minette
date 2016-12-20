using System;
using System.Collections.Generic;
using Minette.Util;

namespace Minette.Application
{
    public class Tagger : ITagger
    {
        public string Url { get; set; }
        public bool Enabled { get; set; }
        public Tagger()
        {
            this.Enabled = false;
            this.Url = @"https://mecab.azurewebsites.net/parse";
        }
        public Tagger(bool enabled)
        {
            this.Enabled = enabled;
            this.Url = @"https://mecab.azurewebsites.net/parse";
        }
        public Tagger(string url)
        {
            this.Enabled = true;
            this.Url = url;
        }
        public List<MecabNode> Parse(string text)
        {
            var ret = new List<MecabNode>();
            if (!this.Enabled)
            {
                return ret;
            }
            var req = new Dictionary<string, string>();
            req.Add("text", text);
            try
            {
                var jsonString = Http.Post(this.Url, new Dictionary<string, string>() { { "text", text } });
                var nodes = Json.Decode(jsonString).Nodes;
                foreach (var n in nodes)
                {
                    ret.Add(new MecabNode(n));
                }
            }
            catch (Exception ex)
            {
                Console.Write("ParseError : " + ex.Message + "\n" + ex.StackTrace);
            }
            return ret;
        }
    }
}
