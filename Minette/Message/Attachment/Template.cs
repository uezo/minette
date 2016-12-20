using System.Collections.Generic;

namespace Minette.Message.Attachment
{
    public class Template
    {
        public TemplateType Type { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string ItemUrl { get; set; }
        public List<Button> Buttons { get; set; }
        public Template()
        {
            this.Buttons = new List<Button>();
        }
        public Template(TemplateType type)
        {
            this.Type = type;
            this.Buttons = new List<Button>();
        }
        public void AddButton(ButtonType type, string title, string data)
        {
            if (this.Type == TemplateType.Confirm)
            {
                this.Buttons.Add(new Button(ButtonType.Postback, title, data));
            }
            else
            {
                this.Buttons.Add(new Button(type, title, data));
            }
        }
        public void AddButton(string title, string data)
        {
            this.Buttons.Add(new Button(ButtonType.Postback, title, data));
        }
    }

}
