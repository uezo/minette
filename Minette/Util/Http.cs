using System.Collections.Generic;

namespace Minette.Util
{
    //UTFのみ、ヘッダー未対応
    public static class Http
    {
        public static string Get(string url, Dictionary<string, string> param)
        {
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
            var ret = "";
            var req = System.Net.WebRequest.Create(url);
            req.Method = "GET";
            var res = req.GetResponse();
            var resStream = res.GetResponseStream();
            var sr = new System.IO.StreamReader(resStream, System.Text.Encoding.UTF8);
            ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }

        public static string Post(string url, Dictionary<string, string> param)
        {
            var wc = new System.Net.WebClient();
            var ps = new System.Collections.Specialized.NameValueCollection();
            foreach(var p in param)
            {
                ps.Add(p.Key, p.Value);
            }
            byte[] resData = wc.UploadValues(url, ps);
            wc.Dispose();
            return System.Text.Encoding.UTF8.GetString(resData);
        }

        public static string PostJson(string url, object data)
        {
            var req = System.Net.WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            var reqJson = Json.Encode(data);
            byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(reqJson);
            req.ContentLength = postDataBytes.Length;
            var reqStream = req.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();
            var webres = req.GetResponse();
            var resStream = webres.GetResponseStream();
            var sr = new System.IO.StreamReader(resStream, System.Text.Encoding.UTF8);
            var ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }
    }
}
