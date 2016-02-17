using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour {

	// Use this for initialization
	public float speed = 1;
	public GameObject bullet;
	protected Vector3 targetPoint;
	public bool isMoving = false;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool isShooting = false;
		if (Input.GetMouseButtonDown (0))
		{
			isMoving = true;
			isShooting = true;
			targetPoint = new Vector3(Camera.main.ScreenToWorldPoint (Input.mousePosition).x,Camera.main.ScreenToWorldPoint (Input.mousePosition).y,0);

		}
		if(isMoving)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed*Time.deltaTime);
			Vector3 _direction = (targetPoint - transform.position);

			float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg-90;
 			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if(isShooting)
			{
				Shot(_direction);
			}
		}



	}

	protected void Shot (Vector3 direction)
	{
		GameObject bul = Instantiate(bullet,transform.position,transform.rotation) as GameObject;
		bul.GetComponent<BulletBehavior>().SetDirection(direction);
		Camera.main.orthographicSize-=0.1f;
		Invoke("ReturnCameraSize",0.1f);
	}

	void ReturnCameraSize()
	{
		Camera.main.orthographicSize+=0.1f;
	}
}
