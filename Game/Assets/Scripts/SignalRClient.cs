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

    // Unity variables
    [SerializeField]
    private Text textStatus;
    [SerializeField]
    private Button buttonConnect;
    [SerializeField]
    private Button buttonStart;
    [SerializeField]
    private Button buttonRetry;

    // SignalR variables
    private static Uri uri = new Uri("http://localhost:5000/signalr/");
    private static GameHub gameHub;
    private static Connection signalRConnection;

    // Other variables
    float searchTimeOut = -1;

    // End of declaration

    /// <summary>
    /// Connect to the SignalR server
    /// </summary>
    public void Connect()
    {
        // Set searchTimeOut to -1
        searchTimeOut = -1;

        // Initialize the connection
        gameHub = new GameHub(ref textStatus);
        signalRConnection = new Connection(uri, gameHub);
        signalRConnection.Open();
        
        signalRConnection.OnConnected += (conn) => 
        {
            Debug.Log("Connect Successfully!");
            textStatus.text = "You connected successfully!\nTap START! to find an opponent...";
 
            // Disable buttonConnect and Enable buttonStart
            buttonConnect.gameObject.SetActive(false);
            buttonRetry.gameObject.SetActive(false);
            buttonStart.gameObject.SetActive(true);
        };
        signalRConnection.OnError += (conn, err) =>
        {
            Debug.Log(err);
            textStatus.text = "Can't connect to the server :(";
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
            textStatus.text = "Sorry! No opponent found :(";
            buttonStart.gameObject.SetActive(false);
            buttonRetry.gameObject.SetActive(true);
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
        private Text textStatus;

        public GameHub(ref Text textStatus) : base("GameHub")
        {
            this.textStatus = textStatus;

            base.On("JoinToOpponent", Joined);
        }

        /// <summary>
        /// Return the join state from server - True: Opponent found, False: Otherwise
        /// </summary>
        private void Joined(Hub hub, MethodCallMessage msg)
        {
            Debug.Log(msg.Arguments[0].ToString());
            bool found = (bool)(msg.Arguments[0]);

            if (found)
                SceneManager.LoadScene(1);
            else
                textStatus.text = "Waiting for an opponent...";
        }
    }
}
