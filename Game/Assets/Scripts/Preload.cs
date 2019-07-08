using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        // Load the connect scene
        SceneManager.LoadScene("Connect");
	}
}
