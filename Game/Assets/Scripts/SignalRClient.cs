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
    ConnectVars connectVars;
    float searchTimeOut = -1;

    // End of declaration

    /// <summary>
    /// Connect to the SignalR server
    /// </summary>
    public void Connect()
    {
        connectVars = GameObject.Find("ConnectVars").GetComponent<ConnectVars>();
        // Set searchTimeOut to -1
        searchTimeOut = -1;

        // Initialize the connection
        gameHub = new GameHub(ref connectVars);
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
        ConnectVars connectVars;

        public GameHub(ref ConnectVars connectVars) : base("GameHub")
        {
            this.connectVars = connectVars;

            // Register callback functions that received from the server
            base.On("JoinToOpponent", Joined);
            base.On("OpponentLeft", Left);
        }

        /// <summary>
        /// Return the join state from server - True: Opponent found, False: Otherwise
        /// </summary>
        private void Joined(Hub hub, MethodCallMessage msg)
        {
            Debug.Log(msg.Arguments[0].ToString());
            bool found = (bool)(msg.Arguments[0]);

            if (found)
                SceneManager.LoadScene("Game");
            else
                connectVars.SetTextStatus("Waiting for an opponent...");
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
    }
}
