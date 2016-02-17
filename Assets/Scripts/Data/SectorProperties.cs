using UnityEngine;
using System.Collections;

public class SectorProperties {
	public float goOut = 0;
	public float goBackEmpty = 0;
	public float goBackFull = 0;
	public float usefulRange = 0;
	public float goBackFromHive = 0;
	public float goBackFromEnemy = 0;
	public bool isForbidden = false;
	public int indexX;
	public int indexY;
	public Vector3 direction;

	public void SetIndexes(int iX, int iY)
	{
		indexX = iX;
		indexY = iY;
	}
}
