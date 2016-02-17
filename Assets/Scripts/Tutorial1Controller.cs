using UnityEngine;
using System.Collections;

public class Tutorial1Controller : MonoBehaviour {

	private LevelController levelController;
	void Start () {
		levelController = GameObject.FindObjectOfType<LevelController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
