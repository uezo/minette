using System;
using System.Collections.Generic;
using Minette.Message;
namespace Minette
{
    public interface IClassifier
    {
        Request Request { get; set; }
        Session Session { get; set; }
        ILogger Logger { get; set; }
        void GetClassified();
    }
    public interface ISessionManager
    {
        Session GetSession(string Id);
        void SaveSession(Session session);
    }
    public interface IUserManager
    {
        User GetUser(string channelUserId);
        void SaveUser(User user);
    }
    public interface ILogger
    {
        void Write(string message);
    }
    public interface IMessageLogger
    {
        DateTime Timestamp { get; set; }
        string Channel { get; set; }
        int TotalTick { get; set; }
        string UserId { get; set; }
        string UserName { get; set; }
        EventType EventType { get; set; }
        MessageType MessageType { get; set; }
        string InputText { get; set; }
        string OutputText { get; set; }
        void Write();
    }
    public interface ITagger
    {
        bool Enabled { get; set; }
        List<MecabNode> Parse(string text);
    }
    public interface IDialogService
    {
        Request Request { get; set; }
        Session Session { get; set; }
        ILogger Logger { get; set; }
        void ProcessRequest();
        Response ComposeResponse();
    }
}
