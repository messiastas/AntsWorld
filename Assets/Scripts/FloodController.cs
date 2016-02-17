using UnityEngine;
using System.Collections;

public class FloodController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x<24) 	transform.Translate(new Vector3(0.005f,0,0),Space.World);
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag=="ant")
		{
			other.GetComponentInParent<AntBehavior>().DieWithBlood();
		}
	}
}
