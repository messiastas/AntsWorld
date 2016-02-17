using UnityEngine;
using System.Collections;

public class ParticlesAutoDelete : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);
	}
}
