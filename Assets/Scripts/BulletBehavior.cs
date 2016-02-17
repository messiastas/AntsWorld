using UnityEngine;
using System.Collections;

public class BulletBehavior : MonoBehaviour {


	// Use this for initialization
	public Vector3 direction;
	public float speed = 20;
	public float damage = 1;
	protected float angle;
	void Start () {
		
	}

	public void SetDirection(Vector3 newDir)
	{
		direction = newDir;
		angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg-90;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = Vector3.MoveTowards(transform.position, transform.position+direction, speed*Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag=="ant" )
		{
			other.gameObject.transform.parent.gameObject.GetComponent<AntBehavior>().GetHit(damage);
			Destroy(gameObject);
		}
	}
}
