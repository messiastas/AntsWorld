using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiveController : MonoBehaviour {
	public enum Pheromones {ALL,USEFUL,DANGER,ATTACK,SCOUT};

	public List<List<SectorProperties>> sectors = new List<List<SectorProperties>>();
	public int antsAlive = 0;
	public GameObject antClass;
	public GameObject antSoldierClass;
	public GameObject antScoutClass;
	public Color antColor;
	public int hiveFraction;
	public float hiveSpeed =1;
	public float hiveRange = 5;
	public float hiveMaxCargo = 1;
	public int hiveLifetime = 10;
	public float hiveEyesight = 1;
	public float hiveHealth = 1;
	public float hiveDamage = 1;
	public float hiveRepair = 1;
	public float soldierChance=0.2f;
	public float scoutChance=0.1f;
	public bool isActive = true;
	public float healthToCapture = 10f;
	public float currentHealth = 10f;
	public GameObject track;
	public GameObject trackHive;
	public GameObject trackEnemy;

	public GameObject flagWorker;
	public GameObject flagSoldier;
	public GameObject flagScout;
	public bool isFlagWorker = false;
	public bool isFlagSoldier = false;
	public bool isFlagScout = false;

	public float storage = 0;

	public bool wasBoris = false;
	public bool needBoris = false;
	public Light borisLight;

	public delegate void StorageAction();
	public event StorageAction OnStorageChanged;

	public delegate void UnitsAction();
	public event StorageAction OnUnitsChanged;

	void Start () {
		CreateSectors ();
		ChangeActivity(isActive,hiveFraction,antColor);

	}

	public void ChangeActivity (bool newActivity,int newFraction,Color newColor)
	{
		isActive = newActivity;
		hiveFraction = newFraction;

		if(isActive)
		{
			antColor = newColor;
			//foreach (Renderer r in GetComponentsInChildren<Renderer> ()) {
				GetComponentsInChildren<Renderer>()[0].material.color = antColor;
			//}

			InvokeRepeating ("CreateAnt", 1f, 1f);
		} else 
		{
			GetComponentsInChildren<Renderer>()[0].material.color = antColor;
			if(IsInvoking())
			{
				CancelInvoke();
			}
		}

	}

	void CreateSectors ()
	{
		for (int i = 0; i <= SharedVars.worldSize; i++) {
			List<SectorProperties> minisectors = new List<SectorProperties> ();
			for (int j = 0; j <= SharedVars.worldSize; j++) {
				minisectors.Add (new SectorProperties ());
				minisectors[j].SetIndexes(i,j);
			}
			sectors.Add (minisectors);
		}
		//Debug.Log (sectors [16] [9].goOut);
		
	}
	
	// Update is called once per frame
	void CreateAnt () {
		if(antsAlive<(2+storage)*5)
		{
			GameObject ant;
			float chance = Random.value;
			if(chance<soldierChance)
			{
				ant = Instantiate(antSoldierClass,transform.position,Quaternion.identity) as GameObject;
				ant.GetComponent<AntSoldierBehavior>().SetBehavior(this.gameObject);

			} else if(chance<soldierChance+scoutChance)
			{
				ant = Instantiate(antScoutClass,transform.position,Quaternion.identity) as GameObject;
				ant.GetComponent<AntScoutBehavior>().SetBehavior(this.gameObject);
			} else
			{
				ant = Instantiate(antClass,transform.position,Quaternion.identity) as GameObject;
				ant.GetComponent<AntBehavior>().SetBehavior(this.gameObject);
			}
			ant.transform.parent = this.transform;
			//ant.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.white;
			foreach(Renderer r in ant.GetComponentsInChildren<Renderer>()){
				r.material.color = antColor;
			}
			ChangeUnits(1);
			//if(needBoris && !wasBoris && Random.value>0.75f)
			//{
			//	ant.GetComponent<AntBehavior>().BecomeBoris(borisLight);
			//	wasBoris = true;
			//}
			//CancelInvoke("CreateAnt");
		}

	}

	public void AntClicked (GameObject ant)
	{
		GameObject.FindObjectOfType<LevelController>().UnitCameraFollowTo(ant);
	}

	public void AddToStorage(UsefulData data)
	{
		storage+=data.weight;
		if(OnStorageChanged != null)
			OnStorageChanged();
	}
	public void SpendFromStorage(float num)
	{
		storage-=num;
		if(OnStorageChanged != null)
			OnStorageChanged();
	}

	public void ChangeUnits(int dif)
	{
		antsAlive+=dif;
		if(OnUnitsChanged != null)
			OnUnitsChanged();
	}

	public void PutPheromone(Vector3 point, Pheromones signal, float price)
	{
			int sectorX = Mathf.RoundToInt (point.x);
			int sectorY = Mathf.RoundToInt (point.y);
			switch (signal)
			{
			case Pheromones.ALL:
				if(PutUsefulPheromone(sectorX,sectorY,10,10) || PutHivePheromone(sectorX,sectorY,10)||PutEnemyPheromone(sectorX,sectorY,10))
				{
					SpendFromStorage(price);
				}

				break;
			case Pheromones.USEFUL:
				if(PutUsefulPheromone(sectorX,sectorY,20,20))
				{
					SpendFromStorage(price);
				}
				break;
			case Pheromones.ATTACK:
				if(PutEnemyPheromone(sectorX,sectorY,50))
				{
					SpendFromStorage(price);
				}
				break;
			}
			

	}

	public void RemoveFlag(Pheromones signal)
	{
		switch (signal)
		{
		case Pheromones.ALL:
			flagWorker.transform.position = new Vector3(0,0,-1);
			flagSoldier.transform.position = new Vector3(0,0,-1);
			flagScout.transform.position = new Vector3(0,0,-1);
			break;
		case Pheromones.USEFUL:
			flagWorker.transform.position = new Vector3(0,0,-1);
			isFlagWorker = false;
			break;
		case Pheromones.ATTACK:
			flagSoldier.transform.position = new Vector3(0,0,-1);
			isFlagSoldier = false;
			break;
		case Pheromones.SCOUT:
			flagScout.transform.position = new Vector3(0,0,-1);
			isFlagScout = false;
			break;
		}


	}

	public void PutFlag(Vector3 point, Pheromones signal, float price)
	{
		int sectorX = Mathf.RoundToInt (point.x);
		int sectorY = Mathf.RoundToInt (point.y);
		switch (signal)
		{
		case Pheromones.ALL:
			flagWorker.transform.position = new Vector3(sectorX,sectorY,0);
			flagSoldier.transform.position = new Vector3(sectorX,sectorY,0);
			flagScout.transform.position = new Vector3(sectorX,sectorY,0);
			break;
		case Pheromones.USEFUL:
			flagWorker.transform.position = new Vector3(sectorX,sectorY,0);
			break;
		case Pheromones.ATTACK:
			flagSoldier.transform.position = new Vector3(sectorX,sectorY,0);
			isFlagSoldier = true;
			break;
		case Pheromones.SCOUT:
			flagScout.transform.position = new Vector3(sectorX,sectorY,0);
			break;
		}
		SpendFromStorage(price);


	}
	
	public bool PutUsefulPheromone(int sectorX, int sectorY,float amount=1,float usefulRange=1)
	{
		if(sectors [sectorX] [sectorY].goBackFull==0)
		{
			sectors [sectorX] [sectorY].goBackFull+=amount;
			GameObject trackSphere = Instantiate(track, new Vector3 (sectorX, sectorY, 0f),Quaternion.identity) as GameObject;
			trackSphere.GetComponent<Renderer>().material.color = antColor;
			trackSphere.transform.parent = transform;
			return true;
		}
		sectors [sectorX] [sectorY].goBackFull+=amount;
		sectors [sectorX] [sectorY].usefulRange+=usefulRange;
		return false;
	}
	
	public bool PutEnemyPheromone(int sectorX, int sectorY,float amount=1)
	{
		if(sectors [sectorX] [sectorY].goBackFromEnemy==0)
		{
			sectors [sectorX] [sectorY].goBackFromEnemy+=amount;
			GameObject trackSphere = Instantiate(trackEnemy, new Vector3 (sectorX-0.25f, sectorY, 0f),Quaternion.identity) as GameObject;
			trackSphere.GetComponent<Renderer>().material.color = antColor;
			trackSphere.transform.parent = transform;
			return true;
		}
		sectors [sectorX] [sectorY].goBackFromEnemy+=amount;
		return false;
	}
	
	public bool PutHivePheromone(int sectorX, int sectorY,float amount=1)
	{
		if(sectors [sectorX] [sectorY].goBackFromHive==0)
		{
			sectors [sectorX] [sectorY].goBackFromHive+=amount;
			GameObject trackSphere = Instantiate(trackHive, new Vector3 (sectorX+0.25f, sectorY, 0f),Quaternion.identity) as GameObject;
			trackSphere.GetComponent<Renderer>().material.color = antColor;
			trackSphere.transform.parent = transform;
			return true;
		}
		sectors [sectorX] [sectorY].goBackFromHive+=amount;
		return false;
	}

	public void GetCaptured(int captureFraction, float captureDamage, Color newColor)
	{
		//Debug.Log ("HIVE "+gameObject.name+" effected by fraction" + captureFraction + ", current health: " + currentHealth);
		if(captureFraction!=hiveFraction)
		{
			currentHealth-=captureDamage;
			Color sendColor = Color.Lerp(antColor, Color.grey, (healthToCapture-currentHealth)/healthToCapture);
			ChangeActivity(false, hiveFraction,sendColor);
			if(currentHealth<=0)
			{
				currentHealth=0;
				hiveFraction = captureFraction;
			}
		} else 
		{

			currentHealth+=captureDamage;
			if(currentHealth>=healthToCapture)
			{
				currentHealth = healthToCapture;
				ChangeActivity(true,captureFraction,newColor);
			} else 
			{
				Color sendColor = Color.Lerp(Color.grey, newColor, currentHealth/(healthToCapture*1.5f));
				GetComponentsInChildren<Renderer>()[0].material.color = sendColor;
			}

		}
	}

	void OnMouseDown()
	{
		//Debug.Log("ANT CLICKED");
		if (hiveFraction==0)
		{
			GameObject.FindObjectOfType<LevelController>().ChangeHive(gameObject);
		}
	}
}
