using UnityEngine;

public class ButtonOnClick : MonoBehaviour 
{
    /// <summary>
    /// When you click on connect button
    /// </summary>
    public void ButtonConnect()
    {
        GameObject.Find("SignalRClient").GetComponent<SignalRClient>().Connect();
    }

    // When you click on start button
    public void ButtonStart()
    {
        GameObject.Find("SignalRClient").GetComponent<SignalRClient>().SearchOpponent();
    }
}
