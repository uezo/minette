Minette
=========

Minette is a Bot "APPLICATION" Framework for LINE, Facebook, Twitter, Command Line and other Messaging Platform.

MinetteはBot「アプリケーション」フレームワークで、LINEやFacebook、Twitterに加え、コマンドラインやその他あらゆるメッセージサービスと接続することができます。

## Install
Now available on [NuGet](https://www.nuget.org/packages/Minette)
```
PM> Install-Package Minette
```


## Features
- Easy to develop
- High reusability
- Session management
- Built-in Japanese annotation
- Chatting ready
- Sample adaptors (includes LINE)

### Easy to develop
You can initialize the Minette just writing 1 line. 
```csharp
//Initialize the Minette(Core is also initialized in the TextAdaptor)
var adaptor = new Minette.Channel.Text.Adaptor();

//Conversasion loop
while (true)
{
    Console.Write("user> ");
    var text = Console.ReadLine();
    Console.WriteLine("minette> " + adaptor.ProcessRequest(text, "user_id_like_12345"));
}
```

### High reusability
You can create another DialogService by extend or override an existing DialogService.
For example, you already have a DialogService that specify the station, you can reuse it and can make "Train Timetable", "Find Restaurant" and any other DialogServices that use station name.


### Session Management
Minette has a data store that enables your bot to continue conversasion accross the requests like HTTP Session.
Default session manager uses the SQL Database but you can override and change to any database you like.

### Built-in Japanese annotation
If you are to make a Japanese speaking bot, you can annotate the input message by Minette MeCab Webservice. Just giving the tagger enabled parameter then you can pick the keywords(e.g.nouns) from the message very easily.
```csharp
//Init the core
var minette = new Minette.Core();
//Set enabled/disabled Japanese Tagger
minette.Tagger.Enabled = true;
```

### Chatting ready
Minette has a DialogService for chatting based on NTT Docomo API. Just input your API Key and set the DialogService then your bot speaks like a human.
```csharp
Session.DialogService = new Minette.Application.ChatDialog("<Input your API Key>");
```

### Sample adaptors
- LINE
- Facebook Messenger
- Text
- Twitter(not included in Minnete.dll because it requires CoreTweet)

Because catching up the changes of API of all channels quickly is very hard for me, so Minette provides the channel adaptors as samples.



## Architecture

![Minette Architecture](http://uezo.net/minette/minettearchitecture.png "Minette Architecture")

- Core : Core of this framework. Runs the flow by using Classifier and DialogServices
- Classifier : Classify the topic of user message and setup the proper DialogService to process the message
- DialogService(s) : Implements of the flow of dialog. This class has following 2 methods:
- ProcessRequest : Setting up the session data by using user input message
- ComposeResponse : Making response message by using session data
- Adaptor : Maps the request / response from channel dependent to Minette common
- Request / Response : Channel independent request / response
- Channel Request / Response : Request / Response of each channels
- Tagger : Annotates Japanese morpheme
- SessionManager : Gets/Saves Session from the data store like RDBMS
- UserManager : Gets/Saves User from the data store like RDBMS by User Id of the channel request
- Session : Stored data across requests like HTTP Session
- User : User information
- Data : One of session data that depends on DialogService
- Logger : Simple debug logger that writes just message body and it’s timestamp


## Sample Codes
First of all create a new Command Line App Project and install Minette from nuget.

### Echo Bot
Add this code to the Main in Program.cs
```csharp
//Initialize the Minette(Core is also initialized in the TextAdaptor)
var adaptor = new Minette.Channel.Text.Adaptor();

//Conversasion loop
while (true)
{
    Console.Write("user> ");
    var text = Console.ReadLine();
    Console.WriteLine("minette> " + adaptor.ProcessRequest(text, "user_id_like_12345"));
}
```


### Hello, Minette
Add this class to your project. This is a classifier class that sets "Hello, Minette!" into session data in every case.
```csharp
namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        public override void GetClassified()
        {
            Session.Data = "Hello, Minette!";
        }
    }
}
```

### Switching by input message
Modify the GetClassified method like below
```csharp
namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        public override void GetClassified()
        {
            if (Request.Text == "morning")
            {
                Session.Data = "Good morning";
            }
            else if (Request.Text == "evening"){
                Session.Data = "Good evening";
            }
            else
            {
                Session.Data = "Hello";
            }
        }
    }
}
```

### Chatting Bot
Just chatting is very easy. You need to get Docomo API key before running this code.
```csharp
namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        public override void GetClassified()
        {
            Session.DialogService = new Minette.Application.ChatDialog("<Input your API Key>");
        }
    }
}
```

### Use your own DialogService
First, add this DialogService class to your project
```csharp
using Minette;
using Minette.Message;

namespace SampleApp
{
    public class DialogService : Minette.Application.DialogService
    {
        public override void ProcessRequest()
        {
            //multiple by 3 when the message is a number
            var inputNumber = int.Parse(Request.Text);
            //set the number to session data
            Session.Data = inputNumber * 3;
        }

        public override Response ComposeResponse()
        {
            //init response
            var res = new Response(Request.MessageId, ResponseType.Text);
            //compose message by using session data
            res.Text = "input value * 3 is" + ((int)Session.Data).ToString();
            return res;
        }
    }
}
```
Then modify classifier like this
```csharp
namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        public override void GetClassified()
        {
            Session.DialogService = new DialogService();
        }
    }
}
```

### Switch the DialogServices by input message
Change the classifier like below
```csharp
namespace SampleApp
{
    public class Classifier : Minette.Application.Classifier
    {
        public override void GetClassified()
        {
            //message is a number -> Your DialogService
            if (request.Text.Contains("1"))
            {
                Session.DialogService = new DialogService();
            }
            //else case -> Chatting
            else
            {
                Session.DialogService = new Minette.Application.ChatDialog("<Input your API Key>");
            }
        }
    }
}
```


## License

This software is licensed under the Apache v2 License.
