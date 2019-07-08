using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;

public class SignalRClient : MonoBehaviour 
{
    // Declaration the variables

    // SignalR variables
    private static Uri uri = new Uri("http://localhost:5000/signalr/");
    private static GameHub gameHub;
    private static Connection signalRConnection;

    // Other variables
    private ConnectVars connectVars;
    private SignalRClient signalRClient;
    private float searchTimeOut = -1;

    public static string playerName;
    private float[] receivedPosition = new float[2];

    // End of declaration

    /// <summary>
    /// Connect to the SignalR server
    /// </summary>
    public void Connect()
    {
        signalRClient = this;
        connectVars = GameObject.Find("ConnectVars").GetComponent<ConnectVars>();

        // Initialize the connection
        gameHub = new GameHub(ref signalRClient, ref connectVars);
        signalRConnection = new Connection(uri, gameHub);
        signalRConnection.Open();
        
        signalRConnection.OnConnected += (conn) => 
        {
            Debug.Log("Connect Successfully!");
            connectVars.SetTextStatus("You connected successfully!\nTap START! to find an opponent...");
 
            // Disable buttonConnect, buttonRetry and Enable buttonStart
            connectVars.ButtonConnectSetActive(false);
            connectVars.ButtonRetrySetActive(false);
            connectVars.ButtonStartSetActive(true);
        };
        signalRConnection.OnError += (conn, err) =>
        {
            Debug.Log(err);
            connectVars.SetTextStatus("Can't connect to the server :(");
        };
    }

    /// <summary>
    /// Search an alone opponent to connect him
    /// </summary>
    public void SearchOpponent()
    {
        // Call the SearchOpponent function from the server and set a timeout to 60 seconds
        signalRConnection[gameHub.Name].Call("SearchOpponent");
        searchTimeOut = Time.time + 60;
    }

    /// <summary>
    /// Send position information to the server
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="y">The y position</param>
    public void SendTransform(float x, float y)
    {
        signalRConnection[gameHub.Name].Call("SendTransformation", x, y);
    }

    /// <summary>
    /// Get the X that received from the server
    /// </summary>
    public float[] GetReceivedPosition()
    {
        return receivedPosition;
    }

	/// <summary>
    ///  Use this for initialization
	/// </summary>
	void Start () 
    {
        DontDestroyOnLoad(this);
	}
	
	/// <summary>
    /// FixedUpdate is called once per frame
	/// </summary>
	void FixedUpdate () 
    {
        // If search timeout
        if (searchTimeOut != -1 && Time.time > searchTimeOut)
        {
            signalRConnection.Close();

            connectVars.SetTextStatus("Sorry! No opponent found :(");
            connectVars.ButtonStartSetActive(false);
            connectVars.ButtonRetrySetActive(true);

            // Set searchTimeOut to -1
            searchTimeOut = -1;
        }
	}

    /// <summary>
    /// On quit the Unity game
    /// </summary>
    void OnApplicationQuit()
    {
        // Close the signalR connection
        if(signalRConnection.State != ConnectionStates.Closed)
            signalRConnection.Close();
    }

    /// <summary>
    /// A class for connection with server hub
    /// </summary>
    public class GameHub : Hub
    {
        private SignalRClient signalRClient;
        private ConnectVars connectVars;

        public GameHub(ref SignalRClient signalRClient, ref ConnectVars connectVars) : base("GameHub")
        {
            this.signalRClient = signalRClient;
            this.connectVars = connectVars;

            // Register callback functions that received from the server
            base.On("JoinToOpponent", Joined);
            base.On("OpponentLeft", Left);
            base.On("OpponentTransformation", Transformation);
        }

        /// <summary>
        /// Return the join state from server - -1: Opponent not found, Otherwise: Opponent found
        /// </summary>
        private void Joined(Hub hub, MethodCallMessage msg)
        {
            Debug.Log(msg.Arguments[0].ToString());
            int playerId = int.Parse(msg.Arguments[0].ToString());

            if (playerId == -1)
            {
                connectVars.SetTextStatus("Waiting for an opponent...");
            }
            else
            {
                // Set searchTimeOut to -1
                signalRClient.searchTimeOut = -1;
                
                // Set your GameObject name and start the game
                playerName = "Player" + playerId;
                SceneManager.LoadScene("Game");
            }
        }

        /// <summary>
        /// Do some operations when opponent left the game
        /// </summary>
        private void Left(Hub hub, MethodCallMessage msg)
        {
            // Back to the first scene
            Debug.Log("Player Disconnected!");
            SceneManager.LoadScene("Connect");
        }

        /// <summary>
        /// Get the opponent position from the server
        /// </summary>
        private void Transformation(Hub hub, MethodCallMessage msg)
        {
            float receivedX = float.Parse(msg.Arguments[0].ToString());
            float receivedY = float.Parse(msg.Arguments[1].ToString());

            Debug.Log("Received opponent position: X = " + receivedX + ", Y = " + receivedY);
            
            signalRClient.receivedPosition[0] = receivedX;
            signalRClient.receivedPosition[1] = receivedY;
        }
    }
}
