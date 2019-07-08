using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ConnectVars : MonoBehaviour 
{
    // Declaration variables
    [SerializeField]
    private Text textStatus;
    [SerializeField]
    private Button buttonConnect;
    [SerializeField]
    private Button buttonStart;
    [SerializeField]
    private Button buttonRetry;

    /// <summary>
    /// Set a text to the textStatus
    /// </summary>
    /// <param name="value">The input text</param>
    public void SetTextStatus(string value)
    {
        this.textStatus.text = value;
    }

    /// <summary>
    /// Disable or enable the buttonConnect
    /// </summary>
    /// <param name="active">The state of activate</param>
    public void ButtonConnectSetActive(bool active)
    {
        buttonConnect.gameObject.SetActive(active);
    }

    /// <summary>
    /// Disable or enable the buttonStart
    /// </summary>
    /// <param name="active">The state of activate</param>
    public void ButtonStartSetActive(bool active)
    {
        buttonStart.gameObject.SetActive(active);
    }

    /// <summary>
    /// Disable or enable the buttonRetry
    /// </summary>
    /// <param name="active">The state of activate</param>
    public void ButtonRetrySetActive(bool active)
    {
        buttonRetry.gameObject.SetActive(active);
    }
}
