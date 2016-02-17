using UnityEngine;
using System.Collections;

public class CreatureHealth : MonoBehaviour {

	public float health=5;
	public GameObject blood;

	public bool GetDamage(float damage)
	{
		health-=damage;
		if(health<=0)
		{			
			DieWithBlood();
			return true;
		}
		return false;
	}

	public void DieWithBlood()
	{
		Instantiate(blood,this.transform.position+new Vector3(0,0,-5),Quaternion.identity);
		Destroy(GetComponent<BoxCollider2D>());
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.material.color = Color.grey;
		}
	}
}
