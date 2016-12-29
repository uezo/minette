using System;
using System.Collections.Generic;
using Minette.Util;

namespace Minette.Application
{
    /// <summary>
    /// Japanese morpheme anotation engine
    /// </summary>
    public class Tagger : ITagger
    {
        /// <summary>
        /// Endpoint url of annotation service
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Annotate or not
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Create a new Tagger with Minette MeCab Service
        /// </summary>
        public Tagger()
        {
            this.Enabled = false;
            this.Url = @"https://mecab.azurewebsites.net/parse";
        }
        /// <summary>
        /// Create a new enabled Tagger with Minette MeCab Service
        /// </summary>
        public Tagger(bool enabled)
        {
            this.Enabled = enabled;
            this.Url = @"https://mecab.azurewebsites.net/parse";
        }
        /// <summary>
        /// Create a new enabled Tagger with other annotation service
        /// </summary>
        public Tagger(string url)
        {
            this.Enabled = true;
            this.Url = url;
        }
        /// <summary>
        /// Get the list of annotated text
        /// </summary>
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
