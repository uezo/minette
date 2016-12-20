namespace Minette.Message
{
    public enum ButtonType { WebUrl = 1, Postback }
    public enum TemplateType { Confirm = 1, Button }
    public enum MessageType { Text = 1, Image, Video, Audio, Location, Sticker };
    public enum EventType { Message = 1, Follow, Unfollow, Join, Leave, Postback, Beacon };
}
