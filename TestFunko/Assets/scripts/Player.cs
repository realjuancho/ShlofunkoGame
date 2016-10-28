using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

	// Use this for initialization




	void Start () {
	
	}
	
	// Update is called once per frame

	void Update () {

		if(CrossPlatformInputManager.GetButton("Cross") || TouchPadInput.GetButton("Action"))
			triggerAction = true;
		else
			triggerAction = false;
			 

	}



	public enum ActionContext{

		Null
		,Open

	}

	public ActionContext actionContext;

	public bool triggerAction;
	public void SetActionContext(ActionContext action)
	{
		
		actionContext = action;
	}





}
