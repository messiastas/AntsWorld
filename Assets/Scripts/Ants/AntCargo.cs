using UnityEngine;
using System.Collections;

public class AntCargo : MonoBehaviour {

	public int fraction = 0;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag=="useful" )
		{
			if(GetComponentInParent<AntBehavior>().unitType=="scout") Debug.Log ("SCOUT FIND USEFUL");
			GetComponentInParent<AntBehavior>().FindUseful(other.gameObject);
		} else if (other.gameObject.tag=="ant" && other.gameObject.GetComponent<AntCargo>().fraction!=fraction)
		{
			GetComponentInParent<AntBehavior>().FindEnemy(other.gameObject.transform.parent.gameObject);
		} else if (other.gameObject.GetComponent<HiveController>())
		{
			//Debug.Log("HIVE FOUND");
			GetComponentInParent<AntBehavior>().FindHive(other.gameObject.GetComponent<HiveController>());
		} else if (other.gameObject.GetComponent<CreatureHealth>())
		{
			//Debug.Log("HIVE FOUND");
			GetComponentInParent<AntBehavior>().FindCreature(other.gameObject);
		}
	}

//	void OnTriggerStay2D(Collider2D other)
//	{
//		if (other.gameObject.tag=="ant" && other.gameObject.GetComponent<AntCargo>().fraction!=fraction)
//		{
//			GetComponentInParent<AntBehavior>().FindEnemy(other.gameObject.transform.parent.gameObject);
//		}
//	}

	void OnMouseDown()
	{
		//Debug.Log("ANT CLICKED");
		GetComponentInParent<AntBehavior>().GetClicked();
	}
}
