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
    }

}
