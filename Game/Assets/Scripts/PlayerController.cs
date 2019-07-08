using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    private SignalRClient signalRClient;
    float speed = 0.5f;

	/// <summary>
    /// Use this for initialization
	/// </summary>
	void Start () 
    {
        signalRClient = GameObject.Find("SignalRClient").GetComponent<SignalRClient>();
	}
	
	/// <summary>
    /// FixedUpdate is called once per frame
	/// </summary>
	void FixedUpdate () 
    {
        // Check the players
        if(this.name == SignalRClient.playerName) // If this game object is you
        {
            // Movement
            float horizontal = Input.GetAxis("Horizontal") * speed;
            float vertical = Input.GetAxis("Vertical") * speed;
            transform.position += new Vector3(horizontal, vertical);

            // Send the information to the server
            signalRClient.SendTransform(transform.position.x, transform.position.y);
        }
        else // If this game object is your opponent
        {
            // Received the information from the server and set to this game object
            float[] receivedPosition = signalRClient.GetReceivedPosition();
            float horizontal = receivedPosition[0];
            float vertical = receivedPosition[1];

            transform.position = new Vector2(Mathf.Lerp(transform.position.x, horizontal, 1f), Mathf.Lerp(transform.position.y, vertical, 1f));
        }
	}
}
