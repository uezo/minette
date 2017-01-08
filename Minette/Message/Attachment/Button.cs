namespace Minette.Message.Attachment
{
    public class Button
    {
        public ButtonType Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Payload { get; set; }
        public Button()
        {
        }
        public Button(ButtonType type, string title, string data)
        {
            this.Type = type;
            this.Title = title;
            if (type == ButtonType.WebUrl)
            {
                this.Url = data;
            }
            else
            {
                this.Payload = data;
            }
        }
    }
}
