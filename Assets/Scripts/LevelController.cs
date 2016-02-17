using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelController : MonoBehaviour
{
	public enum PossibleActions
	{
NOTHING,
PATHFORWORK,
PATHFORATTACK,
PATHFORSCOUT,
PLACEWORM}

	;

	public enum PriceType
	{
CONSTANT,
DISTANCE}

	;

	public Camera unitCamera;
	public GameObject currentPlayerHive;
	public Light hivePointer;
	public GameObject flagWorker;
	public GameObject flagSoldier;
	public GameObject flagScout;

	private GameObject unitToFollow;
	private UnitInfoPanel unitPanel;
	private TopPanelController topPanel;
	private Camera mainCamera;

	private PossibleActions currentAction = PossibleActions.NOTHING;
	private PriceType priceType = PriceType.DISTANCE;
	private float cost = 0;


	void Start ()
	{
		unitPanel = GameObject.FindObjectOfType<UnitInfoPanel> ();
		topPanel = GameObject.FindObjectOfType<TopPanelController> ();
		//timeSlider.onValueChanged.AddListener(TimeScaleUpdate);

		currentPlayerHive.GetComponent<HiveController> ().OnStorageChanged += StorageUpdate;
		currentPlayerHive.GetComponent<HiveController> ().OnUnitsChanged += UnitsUpdate;
		mainCamera = Camera.main;
		ChangeHive (currentPlayerHive);
		currentPlayerHive.GetComponent<HiveController> ().flagScout = flagScout;
		currentPlayerHive.GetComponent<HiveController> ().flagWorker = flagWorker;
		currentPlayerHive.GetComponent<HiveController> ().flagSoldier = flagSoldier;
		HiveController[] hives = GameObject.FindObjectsOfType<HiveController> ();
		foreach (HiveController hive in hives) {
				hive.flagScout = flagScout;
				hive.flagWorker = flagWorker;
				hive.flagSoldier = flagSoldier;
		}
	}

	public void UnitCameraFollowTo (GameObject who)
	{
		if (currentAction == PossibleActions.NOTHING) {
			unitCamera.transform.position = who.transform.position + new Vector3 (0, 0, -10);
			unitToFollow = who;
			unitPanel.UnitSelected (unitToFollow);
		}

	}

	void Update ()
	{
		if (unitToFollow) {
			unitCamera.transform.position = unitToFollow.transform.position + new Vector3 (0, 0, -10);
		}
		if (currentAction != PossibleActions.NOTHING) {
			if (Input.GetMouseButtonDown (0) && Input.mousePosition.y < Screen.height - 40) {
				if (currentAction == PossibleActions.PATHFORATTACK) {
					CheckAttackFlagClick (cost);
				} else if (currentAction == PossibleActions.PATHFORWORK) {
					CheckWorkFlagClick (cost);
				}
			}
		}

		/*if(currentAction==PossibleActions.PATHFORWORK && CalculatePosition(cost))
		{
			CheckPathWorkClick(cost);
		} else if(currentAction==PossibleActions.PATHFORATTACK && CalculatePosition(cost))
		{
			CheckPathAttackClick(cost);
		}*/
	}

	private void CheckAttackFlagClick (float price)
	{
		HiveController[] hives = GameObject.FindObjectsOfType<HiveController> ();
		foreach (HiveController hive in hives) {
			if (hive.hiveFraction == currentPlayerHive.GetComponent<HiveController> ().hiveFraction) {
				hive.PutFlag (mainCamera.ScreenToWorldPoint (Input.mousePosition), HiveController.Pheromones.ATTACK, price);
			}
		}
		UserActionChanged (PossibleActions.NOTHING);
	}

	private void CheckWorkFlagClick (float price)
	{
		HiveController[] hives = GameObject.FindObjectsOfType<HiveController> ();
		foreach (HiveController hive in hives) {
			if (hive.hiveFraction == currentPlayerHive.GetComponent<HiveController> ().hiveFraction) {
				hive.PutFlag (mainCamera.ScreenToWorldPoint (Input.mousePosition), HiveController.Pheromones.USEFUL, price);
			}
		}
		UserActionChanged (PossibleActions.NOTHING);
	}

	private void CheckScoutFlagClick (float price)
	{
		HiveController[] hives = GameObject.FindObjectsOfType<HiveController> ();
		foreach (HiveController hive in hives) {
			if (hive.hiveFraction == currentPlayerHive.GetComponent<HiveController> ().hiveFraction) {
				hive.PutFlag (mainCamera.ScreenToWorldPoint (Input.mousePosition), HiveController.Pheromones.SCOUT, price);
			}
		}
		UserActionChanged (PossibleActions.NOTHING);
	}

	private void CheckPathWorkClick (float price)
	{
		currentPlayerHive.GetComponent<HiveController> ().PutPheromone (mainCamera.ScreenToWorldPoint (Input.mousePosition), HiveController.Pheromones.USEFUL, price);
	}

	private void CheckPathAttackClick (float price)
	{
		currentPlayerHive.GetComponent<HiveController> ().PutPheromone (mainCamera.ScreenToWorldPoint (Input.mousePosition), HiveController.Pheromones.ATTACK, price);
	}

	private bool CalculatePosition (float price)
	{
		if (Input.GetMouseButton (0) && Input.mousePosition.y < Screen.height - 40 && price <= currentPlayerHive.GetComponent<HiveController> ().storage) {
			return true;
		} 
		return false;
	}

	public void TimeScaleUpdate (float value)
	{
		Time.timeScale = value;
	}

	public void SoldierRateUpdate (float value)
	{
		currentPlayerHive.GetComponent<HiveController> ().soldierChance = value;
	}

	public void ScoutsRateUpdate (float value)
	{
		currentPlayerHive.GetComponent<HiveController> ().scoutChance = value;
	}

	void StorageUpdate ()
	{
		topPanel.StorageUpdate (currentPlayerHive.GetComponent<HiveController> ().storage.ToString ());
		//storageText.text = currentPlayerHive.GetComponent<HiveController>().storage.ToString();
	}

	void UnitsUpdate ()
	{
		topPanel.UnitsUpdate (currentPlayerHive.GetComponent<HiveController> ().antsAlive.ToString ());
	}

	public void UserActionChanged (PossibleActions newAction)
	{
		HiveController[] hives = GameObject.FindObjectsOfType<HiveController> ();
		HiveController.Pheromones currentPheromones = HiveController.Pheromones.DANGER;
		switch (newAction) {
		case PossibleActions.NOTHING:
			cost = 0;
			currentAction = newAction;
			priceType = PriceType.CONSTANT;
			if (unitToFollow != null) {
				unitToFollow = null;
				unitPanel.UnitUnselected ();
			}
			Debug.Log ("Action Changed " + newAction);
			return;
			break;
		case PossibleActions.PATHFORWORK:
			currentPheromones = HiveController.Pheromones.USEFUL;
			cost = 5;
			break;
		case PossibleActions.PATHFORATTACK:
			currentPheromones = HiveController.Pheromones.ATTACK;
			cost = 10;
			break;
		case PossibleActions.PATHFORSCOUT:
			currentPheromones = HiveController.Pheromones.SCOUT;
			cost = 1;
			break;
		}
		foreach (HiveController hive in hives) {
			if (hive.hiveFraction == currentPlayerHive.GetComponent<HiveController> ().hiveFraction) {
				hive.RemoveFlag (currentPheromones);
			}
		}

		currentAction = newAction;
		priceType = PriceType.CONSTANT;
		if (unitToFollow != null) {
			unitToFollow = null;
			unitPanel.UnitUnselected ();
		}
		Debug.Log ("Action Changed " + newAction);
	}

	public void ChangeHive (GameObject hive)
	{
		if (currentAction == PossibleActions.NOTHING) {
			currentPlayerHive.GetComponent<HiveController> ().OnStorageChanged -= StorageUpdate;
			currentPlayerHive.GetComponent<HiveController> ().OnUnitsChanged -= UnitsUpdate;
			currentPlayerHive = hive;
			hivePointer.transform.position = currentPlayerHive.transform.position;
			currentPlayerHive.GetComponent<HiveController> ().OnStorageChanged += StorageUpdate;
			currentPlayerHive.GetComponent<HiveController> ().OnUnitsChanged += UnitsUpdate;
			StorageUpdate ();
			UnitsUpdate ();
		}
		
	}
}
