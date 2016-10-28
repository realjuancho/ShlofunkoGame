using UnityEngine;
using System.Collections;

public class TouchPadInput : MonoBehaviour {

	public GameObject originPoint;
	public GameObject referenceGameObject;
	public GameObject arrowPoint;

	public TouchButtons[] buttons;

	public Player playerToControl; 

	void Awake()
	{
		if(!playerToControl) 
		{
			playerToControl = GameObject.FindObjectOfType<Player>();
		}
	}
		
	void Update()
	{

		//CheckForButtonPress();

		CheckForMovementTouch();
	}

	bool isMovementFingerOn;
	int lastTouchCount = 0;

	void CheckForMovementTouch()
	{
		#region MovementInput

	    if(Input.touches.Length > 0)
		{

			Touch touch = Input.GetTouch(0);
			Vector3 fingerPos = touch.position;

		//TODO: Mouse emulation of touch
//		if(Input.GetMouseButton(0))
//		{
//			Vector3 touch = Input.mousePosition;
//			Vector3 fingerPos = touch;

			Camera	cam = Camera.main;		
			//Posicionar el vector en el lugar donde el usuario tiene el dedo mas .5 unidades para ver el objeto

			fingerPos.z = cam.nearClipPlane + 0.5f;
		
            Vector3 objPos = cam.ScreenToWorldPoint (fingerPos);

			//Verificar si el usuario comenzó a presionar el dedo
			if(lastTouchCount == 0)
			{
				
				fingerPos.z = cam.nearClipPlane + 0.8f;
				Vector3 arrowPos = cam.ScreenToWorldPoint(fingerPos);
			
					
				arrowPoint.transform.position = arrowPos;
				arrowPoint.transform.LookAt(objPos);

				referenceGameObject.transform.position = arrowPos;
				referenceGameObject.transform.LookAt(objPos);

			}


			originPoint.transform.position = objPos;

			//TODO: Touchpad
			if(touch.position.x < Screen.width /2 && touch.position.x > 0 &&
				touch.position.y < Screen.height && touch.position.y > 0)
//			if(touch.x < Screen.width /2 && touch.x > 0 &&
//				touch.y < Screen.height && touch.y > 0)
			{
				isMovementFingerOn = true;
				arrowPoint.transform.LookAt(originPoint.transform.position);

			}
			else
			{
				isMovementFingerOn = false;
			}
		}
		else
		{
			isMovementFingerOn = false;
			lastTouchCount = 0;
		}

		ActivateObjects(isMovementFingerOn);

		//TODO: Touchpad
		lastTouchCount = Input.touches.Length;

		//TODO: Mouse
		//lastTouchCount = Input.GetMouseButton(0) ? 1 : 0 ;

		if(originPoint)
		{
			if(originPoint.activeInHierarchy && arrowPoint.activeInHierarchy)
			{


//				Debug.DrawRay(referenceGameObject.transform.position, referenceGameObject.transform.up, Color.green, 3.0f);
//				Debug.DrawRay(referenceGameObject.transform.position, referenceGameObject.transform.right, Color.red, 3.0f);
//				Debug.DrawRay(referenceGameObject.transform.position, referenceGameObject.transform.forward, Color.blue, 3.0f);
//
//				Debug.DrawRay(arrowPoint.transform.position, arrowPoint.transform.forward, Color.yellow);
//
//
//				Debug.DrawRay(referenceGameObject.transform.position, 
//					Vector3.Cross(arrowPoint.transform.forward, referenceGameObject.transform.up), 
//					Color.magenta);
//				//Debug.Log(offset.magnitude);
//
//				Debug.Log("Dot Product yellow and green" + Vector3.Dot(arrowPoint.transform.forward, referenceGameObject.transform.up));

				Camera cam = Camera.main;
				Vector3 originPosToCam = cam.WorldToScreenPoint(originPoint.transform.position);
				Vector3 arrowPosToCam = cam.WorldToScreenPoint(arrowPoint.transform.position);

				Vector3 _2dOffset = originPosToCam - arrowPosToCam;
				float _2dDistance = _2dOffset.magnitude;

				//Debug.Log("2dDistance" + _2dDistance);

				Vector3 _2dDirection = _2dOffset / _2dDistance;

				float x = _2dDirection.x;
				float y = _2dDirection.y;
				float z = _2dDirection.z;

				Debug.Log("X:" + x + "   Y: " + y +  "   Z: " + z);


				MovementAxis_Horizontal = x ;
				MovementAxis_Vertical = y ;

			}
			else
			{
				MovementAxis_Horizontal = 0.0f;
				MovementAxis_Vertical = 0.0f;
			}
		}
		#endregion


	}

	public static bool GetButton(string buttonName)
	{
		//TODO: Touchpad
		if (Input.touches.Length > 0) {	

			for(int i = 0; i < Input.touches.Length; i++)
			{
				Touch buttonTouch = Input.GetTouch (i);

				//TODO: Mouse
		//		if(Input.GetMouseButton(0))
		//		{
		//			Vector3 buttonTouch = Input.mousePosition;

					Camera cam = Camera.main;

					//TODO: TouchPad
					Ray rayButton = cam.ScreenPointToRay(buttonTouch.position);

					//TODO:
					//Ray rayButton = cam.ScreenPointToRay(buttonTouch);

					RaycastHit buttonHit = new RaycastHit();
					bool buttonHitFound = Physics.Raycast(rayButton, out buttonHit);


					Debug.DrawRay(cam.transform.position, rayButton.direction, Color.yellow, 3.0f);

					if (buttonHitFound) {
					
						TouchButtons touchedButton = buttonHit.collider.gameObject.GetComponent<TouchButtons> ();

						if (touchedButton) {
							
							touchedButton.setTouched (true);

							if(touchedButton.Name == buttonName)
								return true;
						}	
					
					}
			}
		}

		return false;
	}




	void ActivateObjects(bool Activate)
	{

		if(Activate && !originPoint.activeInHierarchy && !arrowPoint.activeInHierarchy)
		{
			originPoint.SetActive(true);
			arrowPoint.SetActive(true);
		}
		else if(!Activate)
		{
			if(originPoint)
			{
				originPoint.SetActive(false);
			}
			if(arrowPoint)
			{
				arrowPoint.SetActive(false);
			}
		}


	}

	public static float MovementAxis_Vertical;
	public static float MovementAxis_Horizontal;


}
