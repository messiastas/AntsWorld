using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour {
	public Slider timeSlider;
	public Slider soldierSlider;
	public Slider scoutsSlider;
	public Text storageText;
	public Text unitsText;

	void Start () {
		timeSlider.onValueChanged.AddListener(TimeScaleUpdate);
		soldierSlider.onValueChanged.AddListener(SoldierRateUpdate);
		scoutsSlider.onValueChanged.AddListener(ScoutsRateUpdate);
	}

	public void TimeScaleUpdate(float value)
	{
		GameObject.FindObjectOfType<LevelController>().TimeScaleUpdate(value);
	}
	
	public void SoldierRateUpdate(float value)
	{
		GameObject.FindObjectOfType<LevelController>().SoldierRateUpdate(value);
	}

	public void ScoutsRateUpdate(float value)
	{
		GameObject.FindObjectOfType<LevelController>().ScoutsRateUpdate(value);
	}
	
	public void StorageUpdate(string num)
	{
		storageText.text = num;
	}

	public void UnitsUpdate(string num)
	{
		unitsText.text = num;
	}
}
