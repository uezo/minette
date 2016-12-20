using System.Collections.Generic;
using Minette.Util;

namespace Minette.WebService.Microsoft
{
    public class OCR
    {
        public string APIKey { get; set; }
        public OCR(string apiKey)
        {
            this.APIKey = apiKey;
        }
        //OCR
        public OCRResponse GetTextFromImage(byte[] imageBytes, string language)
        {
            var ret = new OCRResponse();
            var url = "https://api.projectoxford.ai/vision/v1.0/ocr";
            var param = new Dictionary<string, string>()
            {
                {"language", language},
            };
            var enc = System.Text.Encoding.UTF8;
            if (param.Count > 0)
            {
                url += "?";
                foreach (var p in param)
                {
                    url += p.Key + "=" + System.Web.HttpUtility.UrlEncode(p.Value) + "&";
                }
                if (url.EndsWith("&"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }
            var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            req.Headers.Add("Ocp-Apim-Subscription-Key", this.APIKey);
            req.ContentLength = imageBytes.Length;

            var reqStream = req.GetRequestStream();
            reqStream.Write(imageBytes, 0, imageBytes.Length);
            reqStream.Close();
            var res = (System.Net.HttpWebResponse)req.GetResponse();

            var resStream2 = res.GetResponseStream();
            var sr = new System.IO.StreamReader(resStream2, enc);
            var jsonString = sr.ReadToEnd();
            var json = Json.Decode(jsonString);
            sr.Close();
            ret = new OCRResponse(json);
            return ret;
        }
    }
    //OCRのデータ型
    public class OCRResponse
    {
        public string Language { get; set; }
        public List<List<string>> Regeons { get; set; }
        public OCRResponse()
        {
            this.Regeons = new List<List<string>>();
        }
        public OCRResponse(dynamic json)
        {
            this.Language = json.language;
            this.Regeons = new List<List<string>>();
            foreach (dynamic r in json.regions)
            {
                var lines = new List<string>();
                foreach (dynamic l in r.lines)
                {
                    var line = "";
                    foreach (dynamic w in l.words)
                    {
                        line += (string)w.text + " ";
                    }
                    lines.Add(line.Trim());
                }
                this.Regeons.Add(lines);
            }
        }
    }
}
