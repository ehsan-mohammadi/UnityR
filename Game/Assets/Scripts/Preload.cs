using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour 
{
	/// <summary>
    /// Use this for initialization
	/// </summary>
	void Start () 
    {
        // Load the connect scene
        SceneManager.LoadScene("Connect");
	}
}
