using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntBehavior : MonoBehaviour {

	public delegate void DeathAction();
	public event DeathAction OnDeath;
	public delegate void ChangeAction();
	public event ChangeAction OnChangeAction;

	public HiveController hive;
	public float speed=1f;
	public float range=5f;
	public float maxCargo=1f;
	public int lifeTime = 5;
	public float eyesight = 1f;
	public float maxHealth = 1f;
	public float health = 1f;
	public float damage = 1f;
	public float repair = 1f;
	public int fraction = 0;
	public int currentLifetime = 0;
	public float maxUsefulScore = 20;

	public string unitType = "worker";
	public string unitName = "Boris";
	public string unitAction = "Has no idea what he's doing";

	//public GameObject track;

	public GameObject blood;


	public bool isAlive = true;

	protected Vector3 targetPoint;
	protected List<Vector3> walkedPath = new List<Vector3>();
	protected List<Vector3> targetPath = new List<Vector3>();
	protected float walkedRange = 0;
	protected bool isExploring = true;
	protected float currentCargo = 0;
	protected int distanceToUseful = 0;
	protected GameObject cargo;
	protected Light borisLight;
	protected bool wasUsefulFound = false;
	protected bool wasAlienHiveFound = false;
	protected bool wasEnemyFound = false;

	void Start () {

	}
	
	public virtual void SetBehavior(GameObject hiveC)
	{
		hive = hiveC.GetComponent<HiveController>();
		speed = hive.hiveSpeed+Random.Range(-0.5f,1f);
		range = hive.hiveRange;
		maxCargo = hive.hiveMaxCargo;
		fraction = hive.hiveFraction;
		maxHealth = hive.hiveHealth;
		health = maxHealth;
		damage = hive.hiveDamage;
		repair = hive.hiveRepair;
		GetComponentInChildren<AntCargo>().fraction = fraction;
		InvokeRepeating("FindNextSector",0f,0.5f/speed);
	}

	private void FindNextSector()
	{
		if(walkedRange<=range)
		{
			if(walkedPath.Count<1 || walkedPath.IndexOf(new Vector3(Mathf.FloorToInt(transform.position.x),Mathf.FloorToInt(transform.position.y),0f))<walkedPath.Count-1)
			{
				List<SectorProperties> possibleSectors = GetPossibleSectors();
				if(possibleSectors.Count>0)
				{
					if(targetPath.Count<=0)
					{
						int next = Mathf.Clamp(CalculateBestSector(possibleSectors),0,possibleSectors.Count-1);
						targetPoint = new Vector3(possibleSectors[next].indexX, possibleSectors[next].indexY,0f);
					} else 
					{
						targetPoint = targetPath[0];
						targetPath.RemoveAt(0);
					}
					MarkSectorOnGoOut (Mathf.FloorToInt(transform.position.x),Mathf.FloorToInt(transform.position.y));
					walkedPath.Add(new Vector3(Mathf.FloorToInt(transform.position.x),Mathf.FloorToInt(transform.position.y),0f));
				} else 
				{
					targetPoint = transform.position;
					//Debug.Log("possibleSectors.Count=0");
				}
			}
		} else 
		{

			TimeToGoHome ();
		}
	}

	protected virtual void MarkSectorOnGoOut (int posX, int posY)
	{
		if (walkedPath.IndexOf (new Vector3 (posX, posY, 0f)) < 0 && isExploring) {
			hive.sectors [posX] [posY].goOut++;
		}
	}

	protected void MarkSectorIfUsefulFound (int posX, int posY)
	{
		if (!wasUsefulFound) {
			hive.sectors [posX] [posY].goBackEmpty++;
		}
		else {
			hive.PutUsefulPheromone(posX,posY,1f,maxUsefulScore - (distanceToUseful-walkedPath.Count));
			//hive.sectors [posX] [posY].usefulRange += walkedPath.Count;
		}
	}

	protected void MarkSectorIfAlienHiveFound(int posX, int posY)
	{
		if (wasAlienHiveFound) {
			hive.PutHivePheromone(posX,posY,1f);
		}
	}

	protected void MarkSectorIfEnemyFound(int posX, int posY)
	{
		if (wasEnemyFound) {
			hive.PutEnemyPheromone(posX,posY,1f);
		}
	}

	protected virtual int CalculateBestSector(List<SectorProperties> possibleSectors)
	{
		if (Random.value<0.98f)
		{
			float[] balls = {0,0,0,0};
			float maxBalls =0;
			float max2Balls = 0;
			int bestIndex = -1;
			int best2Index = -1;
			for (int i=0;i<possibleSectors.Count;i++)
			{
				balls[i]+=possibleSectors[i].goBackFromHive*3f;
				balls[i]+=possibleSectors[i].goBackFull*possibleSectors[i].usefulRange;
				balls[i]-=possibleSectors[i].goBackEmpty*(0.01f*walkedPath.Count);
				if(balls[i]>maxBalls)
				{
					max2Balls = maxBalls;
					best2Index = bestIndex;
					maxBalls = balls[i];
					bestIndex = i;
				} else if(balls[i]>max2Balls)
				{
					max2Balls = balls[i];
					best2Index = i;
				}
			}
			if(bestIndex>-1)
			{
				if(best2Index>-1)
				{
					if(Random.value<0.35f)
					{
						return best2Index;
					}
				}
				return bestIndex;
			}


		}
		return Random.Range(0,possibleSectors.Count);
	}

	protected void TimeToGoHome ()
	{
		//Debug.Log ("Walked range max");
		//foreach(Vector3 pos in walkedPath){ Debug.Log(pos);}
		targetPoint = transform.position;
		CancelInvoke ("FindNextSector");
		isExploring = false;
		InvokeRepeating ("FindSectorToHive", 1f, 0.5f/speed);
	}

	protected void FindSectorToHive()
	{
		if(walkedPath.Count>1)
		{

			if(Vector3.Distance(walkedPath[walkedPath.Count-1],transform.position)<=0.25f)
			{
				//Debug.Log("reached" + transform.position+ "    ,    "+walkedPath[walkedPath.Count-1]);
				walkedPath.RemoveAt(walkedPath.Count-1);
				MarkSectorIfUsefulFound (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y));
				MarkSectorIfAlienHiveFound (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y));
				MarkSectorIfEnemyFound (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y));

			}
			//Debug.Log(transform.position+ "    ,    "+walkedPath[walkedPath.Count-1]);
			targetPoint = walkedPath[walkedPath.Count-1];
			//Debug.Log("end go");
		} else 
		{
			TimeToExplore ();
		}
	}

	protected void TimeToExplore ()
	{
		//Debug.Log ("RETURNED HOME, GO AGAIN");
		if (cargo)
		{
			hive.AddToStorage(cargo.GetComponent<UsefulData>());
			Destroy (cargo);
			cargo = null;
			maxCargo+=maxCargo*0.25f;
		}
		
		currentCargo = 0;
		currentLifetime++;
		if(currentLifetime>lifeTime)
		{
			DieFromOldness();
		} else 
		{
			transform.position = hive.transform.position;
			CancelInvoke ("FindSectorToHive");
			isExploring = true;
			walkedRange = 0f;
			wasUsefulFound = false;
			wasAlienHiveFound = false;
			wasEnemyFound = false;
			health = maxHealth;
			distanceToUseful = 0;
			range++;
			InvokeRepeating ("FindNextSector", 0f, 0.5f/speed);
			walkedPath.Clear ();
			
			unitAction = "Has no idea what he's doing";
			if(OnChangeAction != null)
				OnChangeAction();
		}

	}



	private List<SectorProperties> GetPossibleSectors ()
	{
		SectorProperties secUp = CheckSector (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y) + 1);
		SectorProperties secDown = CheckSector (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y) - 1);
		SectorProperties secLeft = CheckSector (Mathf.FloorToInt (transform.position.x) - 1, Mathf.FloorToInt (transform.position.y));
		SectorProperties secRight = CheckSector (Mathf.FloorToInt (transform.position.x) + 1, Mathf.FloorToInt (transform.position.y));
		List<SectorProperties> possibleSectors = new List<SectorProperties> ();
		if (!secUp.isForbidden && walkedPath.IndexOf(new Vector3(secUp.indexX,secUp.indexY,0f))<0)
			possibleSectors.Add (secUp);
		if (!secDown.isForbidden && walkedPath.IndexOf(new Vector3(secDown.indexX,secDown.indexY,0f))<0)
			possibleSectors.Add (secDown);
		if (!secLeft.isForbidden && walkedPath.IndexOf(new Vector3(secLeft.indexX,secLeft.indexY,0f))<0)
			possibleSectors.Add (secLeft);
		if (!secRight.isForbidden && walkedPath.IndexOf(new Vector3(secRight.indexX,secRight.indexY,0f))<0)
			possibleSectors.Add (secRight);
		return possibleSectors;
	}

	private SectorProperties CheckSector(int i, int j)
	{
		if(i>=0 && i<hive.sectors.Count && j>=0 && j<hive.sectors[i].Count && !isLastSector(i,j))//(hive.sectors[i][j]!=null)
		{
			return hive.sectors[i][j];
		} else 
		{
			SectorProperties forbidden = new SectorProperties();
			forbidden.isForbidden = true;
			return forbidden;
		}
	}
	private bool isLastSector(int i, int j)
	{
		if(walkedPath.Count>0)
		{
			if(i==walkedPath[walkedPath.Count-1].x || j==walkedPath[walkedPath.Count-1].y)
			{
				return true;
			}
		} 
		return false;
		
	}

	public virtual void FindUseful(GameObject thing)
	{
		if(thing.GetComponent<UsefulData>().weight+currentCargo<=maxCargo)
		{
			currentCargo+=thing.GetComponent<UsefulData>().weight;
			thing.GetComponent<UsefulData>().SetActivity(false);
			thing.transform.parent = this.transform;
			cargo = thing;
			TimeToGoHome ();
			unitAction = "Bringing something useful to home";
			if(OnChangeAction != null)
				OnChangeAction();
		}
		wasUsefulFound = true;
		distanceToUseful = walkedPath.Count;
	}

	public void FindEnemy(GameObject enemy)
	{
		//if(Vector3.Distance(transform.position,enemy.transform.position)<=eyesight)
		//{
			if (enemy.GetComponent<AntBehavior>().GetHit(damage))
			{
				damage+=damage*0.25f;
			}
		wasEnemyFound = true;
		//}
	}

	public void FindCreature(GameObject creature)
	{
		//if(Vector3.Distance(transform.position,enemy.transform.position)<=eyesight)
		//{

		if (creature.GetComponent<CreatureHealth>().GetDamage(damage))
			{
				damage+=damage*0.25f;
			}
		wasEnemyFound = true;
		//}
	}

	public virtual void FindHive(HiveController foundedHive)
	{
		//if(Vector3.Distance(transform.position,foundedHive.transform.position)<=eyesight)
		//{
		if (foundedHive.hiveFraction==fraction )
			{
				if(foundedHive.currentHealth<foundedHive.healthToCapture)
				{
					Debug.Log("WORKER REPAIRS HIVE");
					foundedHive.GetCaptured(fraction,repair,hive.antColor);
					wasAlienHiveFound = true;
				} else if(foundedHive != hive)
				{
					Debug.Log(unitType + " CHANGED HIVE");
					hive.ChangeUnits(-1);
					hive = foundedHive;
					hive.ChangeUnits(1);
					transform.parent = hive.transform;
					TimeToExplore ();
				}
		} else 
		{
			wasAlienHiveFound = true;
			foundedHive.GetCaptured(fraction,damage,hive.antColor);
		}
		//}
	}

	public bool GetHit(float dmg)
	{
		health-=dmg;
		GameObject bloodParticles = Instantiate(blood,this.transform.position+new Vector3(0,0,0),Quaternion.identity) as GameObject;
		bloodParticles.GetComponentInChildren<ParticleSystem>().maxParticles = 10;
		maxHealth = maxHealth*1.1f;
		if(health<=0)
		{
			DieWithBlood();
			return true;
		}
		wasEnemyFound = true;
		return false;
	}

	public void GetClicked()
	{
		//hive.AntClicked(gameObject);

	}

	public void DieWithBlood()
	{
		if(IsInvoking())
		{
			CancelInvoke();
		}

		Instantiate(blood,this.transform.position+new Vector3(0,0,0),Quaternion.identity);
		this.transform.position = this.transform.position+new Vector3(0,0,1);
		isAlive = false;
		Destroy(GetComponentInChildren<AntCargo>().gameObject);
		if(currentCargo>0f)
		{
			cargo.GetComponent<UsefulData>().SetActivity(true);
			cargo.transform.parent = hive.transform;
			cargo = null;
		}
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.material.color = Color.grey;
		}
		if(OnDeath != null)
			OnDeath();
		hive.ChangeUnits(-1);
	}

	public void DieFromOldness()
	{
		if(IsInvoking())
		{
			CancelInvoke();
		}
		isAlive = false;
		if(OnDeath != null)
			OnDeath();
		hive.ChangeUnits(-1);
		Debug.Log ("I just became old and died");
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(isAlive)
		{
			Vector3 oldPos = transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed*Time.deltaTime);
			walkedRange += Vector3.Distance(oldPos,transform.position);
			Vector3 _direction = (targetPoint - transform.position).normalized;
			
			//create the rotation we need to be in to look at the target
			if(_direction!=Vector3.zero)
			{
				Quaternion _lookRotation = Quaternion.LookRotation(_direction);
				
				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * speed*5f);
			}
			if(borisLight!=null)
			{
				borisLight.transform.position = transform.position+ new Vector3(0,0,4.7f);
			}
		}


	}

	public void BecomeBoris(Light bLight)
	{
		targetPath.Add( new Vector3(12,7,0));
		targetPath.Add( new Vector3(13,7,0));
		targetPath.Add( new Vector3(13,6,0));
		targetPath.Add( new Vector3(13,5,0));
		targetPath.Add( new Vector3(12,5,0));
		targetPath.Add( new Vector3(12,4,0));
		targetPath.Add( new Vector3(12,3,0));
		targetPath.Add( new Vector3(11,3,0));
		targetPath.Add( new Vector3(11,2,0));
		targetPath.Add( new Vector3(12,2,0));
		speed = 0.9f;
		GetClicked();
		borisLight = bLight;
		borisLight.transform.position = transform.position + new Vector3(0,0,4.7f);
		//borisLight.transform.parent = transform;
	}
}
