using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SandWormBehavior : MonoBehaviour {

	public float damage=2f;
	public float timeBetwenAttacks=3f;
	public bool isDamaging = false;
	private float lastTimeAttack = -5f;
	//private List<GameObject> ants = new List<GameObject>();

	void Start()
	{
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			//r.material.color = Color.magenta;
			r.material.color = Color.Lerp(r.material.color, Color.magenta, 0.5f);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag=="ant")
		{
			//ants.Add(other.gameObject);
			if(Time.timeSinceLevelLoad>lastTimeAttack+timeBetwenAttacks)
			{
				GetComponent<Animator>().SetTrigger("Attack");
				isDamaging = true;
				lastTimeAttack = Time.timeSinceLevelLoad;
				foreach(Renderer r in GetComponentsInChildren<Renderer>()){
					r.material.color = Color.Lerp(r.material.color, Color.magenta, 0.1f);
				}
			}
		}
	}
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag=="ant")
		{
			//ants.Add(other.gameObject);
			if(isDamaging)
			{
				other.gameObject.GetComponentInParent<AntBehavior>().GetHit(damage);
			}			
		}
	}

	public void EndAttack()
	{
		isDamaging = false;
	}
}
