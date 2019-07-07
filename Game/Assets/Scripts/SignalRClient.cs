using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;

public class SignalRClient : MonoBehaviour 
{
    // Declaration the variables
    // SignalR variables
    private static Uri uri = new Uri("http://localhost:5000/signalr/");
    private static Hub GameHub = new Hub("GameHub");
    private static Connection signalRConnection;

    // Unity variables
    [SerializeField]
    private Text textStatus;
    [SerializeField]
    private Button buttonConnect;
    [SerializeField]
    private Button buttonStart;

    // End of declaration

    public void Connect()
    {
        // Initialize the connection
        signalRConnection = new Connection(uri, GameHub);
        signalRConnection.Open();
        
        // Type something
        signalRConnection.OnConnected += (conn) => 
        {
            Debug.Log("Connect Successfully!");
            textStatus.text = "You connected successfully!\nTap START! to find an opponent...";
 
            // Disable buttonConnect and Enable buttonStart
            buttonConnect.gameObject.SetActive(false);
            buttonStart.gameObject.SetActive(true);
        };
        signalRConnection.OnError += (conn, err) =>
        {
            Debug.Log(err);
            textStatus.text = "Can't connect to the server :(";
        };
    }

    public void Start()
    {
        signalRConnection[GameHub.Name].On()
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}
}
