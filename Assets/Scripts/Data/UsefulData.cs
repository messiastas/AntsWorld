using UnityEngine;
using System.Collections;

public class UsefulData : MonoBehaviour {

	public float weight = 1f;
	public bool isActive = true;
	// Use this for initialization
	void Start () {
		this.transform.GetChild(0).transform.rotation = Random.rotation;
	}
	
	// Update is called once per frame
	public void SetActivity (bool isA) {
		isActive = isA;
		GetComponent<Collider2D>().enabled = isActive;
	}
}
