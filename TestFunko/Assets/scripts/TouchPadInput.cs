using UnityEngine;
using System.Collections;

public class TouchPadInput : MonoBehaviour {

	
	public GameObject originPoint;
	public GameObject referenceGameObject;
	public GameObject arrowPoint;

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
		CheckForMovementTouch();
		ResetButtons();
	}

	bool isMovementFingerOn;
	int lastTouchCount = 0;

	void CheckForMovementTouch()
	{
		#region MovementInput

//	    if(Input.touches.Length > 0)
//		{
//
//			Touch touch = Input.GetTouch(0);
//			Vector3 fingerPos = touch.position;

		if(Input.GetMouseButton(0))
		{
			
			Vector3 touch = Input.mousePosition;
			Vector3 fingerPos = touch;

		 	Camera cam = Camera.main;		
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
			//if(touch.position.x < Screen.width && touch.position.y < Screen.height )
			if(touch.x < Screen.width && touch.x > 0 &&
				touch.y < Screen.height && touch.y > 0)
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
		//lastTouchCount = Input.touches.Length;

		//TODO: Mouse
		lastTouchCount = Input.GetMouseButton(0) ? 1 : 0 ;

		if(originPoint)
		{
			if(originPoint.activeInHierarchy && arrowPoint.activeInHierarchy)
			{
				Vector3 offset =  originPoint.transform.position - arrowPoint.transform.position;
				offset.Normalize();

				Camera cam = Camera.main;




				float distance = offset.magnitude;

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

				Vector3 direction = offset / distance;

				float x = direction.x;
				float y = direction.y;

				MovementAxis_Horizontal = x > 0 ? 1 : -1;
				MovementAxis_Vertical = y > 0 ? 1 : -1;

			}
			else
			{
				MovementAxis_Horizontal = 0.0f;
				MovementAxis_Vertical = 0.0f;
			}
		}
		#endregion


	}

	void ActivateObjects(bool Activate)
	{

		if(Activate && !originPoint.activeInHierarchy && !arrowPoint.activeInHierarchy)
		{
			originPoint.SetActive(true);
			arrowPoint.SetActive(true);

			//0.3f Magic Number: means the origin will be positioned a little bit below, since don't want to catch an unintended crouch position
			//originPoint.transform.position = new Vector3(positionInWorldPoints.x, positionInWorldPoints.y -0.3f, 0.0f);
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


	public void CheckForButtonInput(int ButtonNumber)
	{
		if(ButtonNumber == 1) Button1 = true; else Button1 = false; 
		if(ButtonNumber == 2) Button2 = true; else Button2 = false; 
		if(ButtonNumber == 3) Button3 = true; else Button3 = false;
		if(ButtonNumber == 4) Button4 = true; else Button4 = false;
		if(ButtonNumber == 5) Button5 = true; else Button5 = false;
		if(ButtonNumber == 6) Button6 = true; else Button6 = false;
		if(ButtonNumber == 7) Button7 = true; else Button7 = false;
		if(ButtonNumber == 8) Button8 = true; else Button8 = false;
		if(ButtonNumber == 9) Button9 = true; else Button9 = false;
	}

	void ResetButtons()
	{
		Button1 = false;
		Button2 = false;
		Button3 = false;
		Button4 = false;
		Button5 = false;
		Button6 = false;
		Button7 = false;
		Button8 = false;
		Button9 = false;
	}

	public static float MovementAxis_Vertical;
	public static float MovementAxis_Horizontal;

	public static bool Button1;
	public static bool Button2;
	public static bool Button3;
	public static bool Button4;

	public static bool Button5;
	public static bool Button6;
	public static bool Button7;
	public static bool Button8;
	public static bool Button9;
}
