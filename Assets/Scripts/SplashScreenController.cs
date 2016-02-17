using UnityEngine;
using System.Collections;

public class SplashScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke("LoadLevel",3f);
	}

	void LoadLevel()
	{
		Application.LoadLevel(1);
	}
}
