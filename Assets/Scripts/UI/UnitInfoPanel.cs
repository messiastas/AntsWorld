using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UnitInfoPanel : MonoBehaviour {

	public Text unitName;
	public Text unitAction;

	GameObject unit;
	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup>().alpha=0;
	}
	
	public void UnitSelected(GameObject newUnit)
	{
		if(unit!=null)
		{
			unit.GetComponent<AntBehavior>().OnDeath-=UnitDead;
			unit.GetComponent<AntBehavior>().OnChangeAction-=UnitChangeAction;
			unit = null;
		}
		unit = newUnit;
		unitName.text=unit.GetComponent<AntBehavior>().unitName;
		unitAction.text = unit.GetComponent<AntBehavior>().unitAction;
		GetComponent<CanvasGroup>().alpha=1;
		unit.GetComponent<AntBehavior>().OnDeath+=UnitDead;
		unit.GetComponent<AntBehavior>().OnChangeAction+=UnitChangeAction;
	}

	void UnitDead()
	{
		unitName.text="";
		unitAction.text = "Oh.. Goodbye "+unit.GetComponent<AntBehavior>().unitName;
		unit.GetComponent<AntBehavior>().OnDeath-=UnitDead;
		unit.GetComponent<AntBehavior>().OnChangeAction-=UnitChangeAction;
		Invoke("UnitUnselected",5f);
	}

	void UnitChangeAction()
	{
		unitAction.text = unit.GetComponent<AntBehavior>().unitAction;
	}

	public void UnitUnselected()
	{

		unit = null;
		GetComponent<CanvasGroup>().alpha=0;
	}
}
