namespace Minette.Message.Attachment
{
    public class Sticker
    {
        public string PackageId { get; set; }
        public string StickerId { get; set; }
        public string Description { get; set; }
        public Sticker() { }
        public Sticker(string Description)
        {
            this.Description = Description;
        }
        public Sticker(string PackageId, string StickerId)
        {
            this.PackageId = PackageId;
            this.StickerId = StickerId;
        }
    }
}
