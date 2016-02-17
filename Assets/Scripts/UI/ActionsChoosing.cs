using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionsChoosing : MonoBehaviour {


	void Start () {
	}
	
	public void UserActionChanged(int action)
	{
		LevelController.PossibleActions currentAction = LevelController.PossibleActions.NOTHING;
		switch (action)
		{
		case 0:
			currentAction = LevelController.PossibleActions.NOTHING;
			break;
		case 1:
			currentAction = LevelController.PossibleActions.PATHFORWORK;
			break;
		case 2:
			currentAction = LevelController.PossibleActions.PATHFORATTACK;
			break;
		case 3:
			currentAction = LevelController.PossibleActions.PATHFORSCOUT;
			break;
		case 4:
			currentAction = LevelController.PossibleActions.PLACEWORM;
			break;
		}
		GameObject.FindObjectOfType<LevelController>().UserActionChanged(currentAction);
	}
}
