# UnityR

> Unity3D, SignalR real-time multiplayer

<p align="center"><img src="https://github.com/ehsan-mohammadi/UnityR/blob/master/Images/UnityR_gif.gif"/></p>

**UnityR** is a smaple multiplayer game. This project consist of two main parts: **Server** and **Game**.

Server part made with [SignalR](https://github.com/SignalR/SignalR) technology and Game part is a Unity3D project. Unity game uses [BestHttp](https://assetstore.unity.com/packages/tools/network/best-http-10872) library to connect the SignalR server.

## Server
The server consists of two classes: [`Startup.cs`](https://github.com/ehsan-mohammadi/UnityR/blob/master/Server/Server/Startup.cs) and [`GameHub.cs`](https://github.com/ehsan-mohammadi/UnityR/blob/master/Server/Server/Hubs/GameHub.cs).

Here is an overview of `Stratup.cs`:

```csharp
public class Startup
{
    /// <summary>
    /// Configure the signalR program
    /// </summary>
    public void Configuration(IAppBuilder app)
    {
        // Maps signalR hubs to the app builder pipeline at "/signalr"
        app.MapSignalR();
    }
}
```

`GameHub.cs` enables you to call methods on connected clients from the server. In this class, you define methods that are called by clients. Here is an overview of this class:

```csharp
public class GameHub : Hub
{
    // Players list that save the players connection id and group id
    private static Dictionary<string, string> players = new Dictionary<string, string>();

    /// <summary>
    /// When player join in the game
    /// </summary>
    public override Task OnConnected()
    {
        // Add player to the players list and set the group id to -2
        players.Add(Context.ConnectionId, "-2");

        return base.OnConnected();
    }

    /// <summary>
    /// When player disconnected
    /// </summary>
    public override Task OnDisconnected(bool stopCalled)
    {
        string groupName = players[Context.ConnectionId];

        // Remove player from the players list and send message to other player to tell him the opponent left
        players.Remove(Context.ConnectionId);
        Clients.Group(groupName).OpponentLeft();

        return base.OnDisconnected(stopCalled);
    }
      
    // And other methods...
    ...
}
```

For more information, read [SignalR documentation](https://docs.microsoft.com/en-us/aspnet/signalr/overview/getting-started/introduction-to-signalr).

## Game
The game consist of a main class called [`SignalRClient.cs`](https://github.com/ehsan-mohammadi/UnityR/blob/master/Game/Assets/Scripts/SignalRClient.cs). By this class, the game communicate with the server. Here is an overview of `SignalRClient.cs`:

```csharp
public class SignalRClient : MonoBehaviour 
{
    // SignalR variables
    private static Uri uri = new Uri("http://localhost:5000/signalr/");
    private static GameHub gameHub;
    private static Connection signalRConnection;
    ...
    public void Connect()
    {
        // Initialize the connection
        gameHub = new GameHub();
        signalRConnection = new Connection(uri, gameHub);
        signalRConnection.Open();
        
        signalRConnection.OnConnected += (conn) => 
        {
            Debug.Log("Connect Successfully!");
        };
        ...
    }
    ...
    /// <summary>
    /// A class for connection with server hub
    /// </summary>
    public class GameHub : Hub
    {
        public GameHub() : base("GameHub")
        {
            // Register callback functions that received from the server
            base.On("JoinToOpponent", Joined);
            ...
        }

        /// <summary>
        /// Return the join state from server - -1: Opponent not found, Otherwise: Opponent found
        /// </summary>
        private void Joined(Hub hub, MethodCallMessage msg)
        {
            int playerId = int.Parse(msg.Arguments[0].ToString());

            if (playerId == -1)
                Debug.Log("Waiting for an opponent...");
            else
                Debug.Log("Player joined!");
        }
        ...
    }
}
```

For more information, read [BestHttp documentation](https://docs.google.com/document/d/181l8SggPrVF1qRoPMEwobN_1Fn7NXOu-VtfjE6wvokg/edit).

## Getting started

- Clone a copy of the repo: `git clone "https://github.com/ehsan-mohammadi/UnityR.git"`
- Open the server project with Visual Studio 2013 or above; Then run the server.
- Open the Game project with Unity3D 5.5.1f1 or above and run it.

**Note 1:** Because of the `BestHttp` is a purchased library, I should to ignore this in my repo. So you need to download it from [Unity Asset Store](https://assetstore.unity.com/packages/tools/network/best-http-10872) and extract it in the `Game\Assets\Plugins` directory of the game. (Read [`BestHttp.md`](https://github.com/ehsan-mohammadi/UnityR/blob/master/Game/Assets/Plugins/BestHttp.md) Carefully)

**Note 2:** You can build the game, then open two windows of it to test the server.

## License

UnityR is available to anybody free of charge, under the terms of MIT License (See [LICENSE](../master/LICENSE)).
