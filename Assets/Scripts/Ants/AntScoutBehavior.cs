using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntScoutBehavior : AntBehavior {

	public override void SetBehavior(GameObject hiveC)
	{
		base.SetBehavior(hiveC);
		maxCargo = 0;
		maxHealth = hive.hiveHealth*3f;
		health = maxHealth;
		damage = hive.hiveDamage*0.5f;
		repair = hive.hiveRepair*0.5f;
		range = hive.hiveRange*4;
		unitType = "scout";
	}

	protected override int CalculateBestSector(List<SectorProperties> possibleSectors)
	{
		if (Random.value<0.98f)
		{
			float balls = 0;
			float minBalls =10;
			float min2Balls = 10;
			float min3Balls = 10;
			int bestIndex = -1;
			int best2Index = -1;
			int best3Index = -1;
			int count = 0;
			for (int i=0;i<possibleSectors.Count;i++)
			{
				balls+=possibleSectors[i].goOut;
				//float missedCount = possibleSectors[i].goBackEmpty+possibleSectors[i].goBackFull-possibleSectors[i].goOut;
				//balls[i]+=missedCount*0.2f;
				if(balls<=minBalls)
				{
					min3Balls = min2Balls;
					best3Index = best2Index;
					min2Balls = minBalls;
					best2Index = bestIndex;
					minBalls = balls;
					bestIndex = i;
					count++;
				} else if(balls<=min2Balls)
				{
					min2Balls = balls;
					best2Index = i;
					count++;
				} else if(balls<=min3Balls)
				{
					min3Balls = balls;
					best3Index = i;
					count++;
				} 
			}
			if(bestIndex>-1 && count<4)
			{
				if(best2Index>-1)
				{
					if(best3Index>-1)
					{
						float chance = Random.value;
						if(chance<0.33f)
						{
							return best3Index;
						} else if (chance<0.66f)
						{
							return best2Index;
						} else 
						{
							return bestIndex;
						}
					}
					if(Random.value<0.5f)
					{
						return best2Index;
					}
				}
				return bestIndex;
			}
		}
		return Random.Range(0,possibleSectors.Count);
	}

	public override void FindUseful(GameObject thing)
	{
		
		TimeToGoHome ();
		wasUsefulFound = true;
		distanceToUseful = walkedPath.Count;
	}
}

