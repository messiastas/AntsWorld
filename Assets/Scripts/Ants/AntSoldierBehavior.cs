using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntSoldierBehavior : AntBehavior {

	public override void SetBehavior(GameObject hiveC)
	{
		base.SetBehavior(hiveC);
		maxCargo = maxCargo*0.5f;
		maxHealth = hive.hiveHealth*2f;
		health = maxHealth;
		damage = hive.hiveDamage*2f;
		repair = hive.hiveRepair*0.5f;
		unitType = "soldier";
	}

	protected override void MarkSectorOnGoOut (int posX, int posY)
	{
		//if (walkedPath.IndexOf (new Vector3 (Mathf.FloorToInt (transform.position.x), Mathf.FloorToInt (transform.position.y), 0f)) < 0 && isExploring) {
		//	hive.sectors [Mathf.FloorToInt (transform.position.x)] [Mathf.FloorToInt (transform.position.y)].goOut++;
		//}
	}

	protected override int CalculateBestSector(List<SectorProperties> possibleSectors)
	{
		int bestIndex = -1;
		if(!hive.isFlagSoldier || Random.value>0.95f)
		{


			if (Random.value<0.98f)
			{
				float[] balls = {0,0,0,0};
				float maxBalls =0;
				float max2Balls = 0;

				int best2Index = -1;
				for (int i=0;i<possibleSectors.Count;i++)
				{
					balls[i]+=possibleSectors[i].goBackFromHive*4f;
					balls[i]+=possibleSectors[i].goBackFromEnemy*1f;
					//float missedCount = possibleSectors[i].goBackEmpty+possibleSectors[i].goBackFull-possibleSectors[i].goOut;
					//balls[i]+=missedCount*0.2f;
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
		} else 
		{
			float distance = Vector3.Distance(this.transform.position,hive.flagSoldier.transform.position);
			for (int i=0;i<possibleSectors.Count;i++)
			{
				float possibleDistance = Vector3.Distance(new Vector3(possibleSectors[i].indexX, possibleSectors[i].indexY, 0),hive.flagSoldier.transform.position);
				if(possibleDistance<distance)
				{
					bestIndex = i;
					distance = possibleDistance;
				}
			}
			return bestIndex;
		}
		return Random.Range(0,possibleSectors.Count);
	}

	public override void FindHive(HiveController foundedHive)
	{

		//if(Vector3.Distance(transform.position,foundedHive.transform.position)<=eyesight)
		//{
			if (foundedHive.hiveFraction!=fraction)
			{
				//Debug.Log("SOLDIER ATTACKS HIVE");
				foundedHive.GetCaptured(fraction,damage,hive.antColor);
				wasAlienHiveFound = true;
			} else if(foundedHive.currentHealth<foundedHive.healthToCapture)
				{
					Debug.Log("SOLDIER REPAIRS HIVE");
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
		//}
	}
}
