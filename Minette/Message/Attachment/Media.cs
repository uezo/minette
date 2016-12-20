using System.Collections.Generic;
using System.IO;

namespace Minette.Message.Attachment
{
    public class Media
    {
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public byte[] BinaryData { get; set; }
        public Media()
        {
            this.Url = "";
            this.ThumbUrl = "";
            this.Headers = new Dictionary<string, string>();
        }
        public Media(string Url)
        {
            this.Url = Url;
            this.ThumbUrl = Url;
            this.Headers = new Dictionary<string, string>();
        }
        public void LoadData()
        {
            //画像を取得
            var req = System.Net.WebRequest.Create(this.Url);
            foreach (var h in this.Headers)
            {
                req.Headers.Add(h.Key, h.Value);
            }
            req.Method = "GET";
            var res = req.GetResponse();
            var s = res.GetResponseStream();
            this.BinaryData = ReadBinaryData(s);
            s.Close();
        }
        private static byte[] ReadBinaryData(Stream st)
        {
            byte[] buf = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = st.Read(buf, 0, buf.Length);
                    if (read > 0)
                    {
                        ms.Write(buf, 0, read);
                    }
                    else
                    {
                        break;
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
