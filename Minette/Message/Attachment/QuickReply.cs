namespace Minette.Message.Attachment
{
    public enum QuickReplyType { Text = 1, Location }
    public class QuickReply
    {
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Payload { get; set; }
        public QuickReply() { }
        public QuickReply(string title, string payload)
        {
            this.ContentType = "text";
            this.Title = title;
            this.Payload = payload;
        }
        public QuickReply(string title, string imageUrl, string payload)
        {
            this.ContentType = "text";
            this.Title = title;
            this.ImageUrl = imageUrl;
            this.Payload = payload;
        }
    }
}
