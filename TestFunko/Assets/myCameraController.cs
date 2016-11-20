using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using System.Collections;

public class myCameraController : MonoBehaviour {


	//Contiene el target dentro del scropt de la cámara
	public Transform transform_objetivo;


	public PositionSettings PS_positionSetting = new PositionSettings ();
	public OrbitSettings OS_orbitSetting = new OrbitSettings ();
	public InputSettings IS_inputSetting = new InputSettings ();
	public DebugSettings DS_debugSetting = new DebugSettings ();
	public CollisionHandler CH_collision = new CollisionHandler ();

	Vector3 v3_PosicionObjetivo = Vector3.zero;
	Vector3 v3_destination = Vector3.zero;
	Vector3 v3_adjustedDestination = Vector3.zero;
	Vector3 v3_camVel = Vector3.zero;

	//myCharacterController CC_charController;
	UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter cc_charController;

	float f_vOrbitInput, f_hOrbitInput, f_zoomInput, f_hOrbitSnapInput;



	void Start()
	{
		SetCameraTarget (transform_objetivo);

		f_vOrbitInput = f_hOrbitInput = f_zoomInput = f_hOrbitSnapInput = 0;

		MoveToTarget ();

		CH_collision.Initialize (Camera.main);

		UpdateCollisionPoints ();

	}


	//Aplica el transform del Character Controller como target 
	public void SetCameraTarget(Transform t)
	{
		transform_objetivo = t;
//
//		if (transform_objetivo != null) {
//
////			if (transform_objetivo.GetComponent<myCharacterController> ()) {
////			
////				CC_charController = transform_objetivo.GetComponent<> ();
////
////			} else {
////				Debug.Log ("The camera's target needs a character controller");
////			}
//		
//		} else {
//			Debug.Log ("The camera needs a target");
//		}

	}
		

	void Update()
	{
		GetInput ();

		//ZoomInOnTarget ();

	}




	void FixedUpdate()
	{
		//moving
		MoveToTarget();

//		//rotating
		LookAtTarget();

//		//Orbiting Target
		OrbitTarget ();

		UpdateCollisionPoints ();

	}

	void GetInput(){
		f_vOrbitInput = CrossPlatformInputManager.GetAxisRaw (IS_inputSetting.ORBIT_VERTICAL);
		f_hOrbitInput = CrossPlatformInputManager.GetAxisRaw (IS_inputSetting.ORBIT_HORIZONTAL);
		f_hOrbitSnapInput = CrossPlatformInputManager.GetAxisRaw (IS_inputSetting.ORBIT_HORIZONTAL_SNAP);
		f_zoomInput = CrossPlatformInputManager.GetAxisRaw (IS_inputSetting.ZOOM);
	}



	/// <summary>
	/// Ubica la camara en el objetivo (cambia la posición solamente, pero no la rotación)
	/// </summary>
	void MoveToTarget()
	{

		//ubica un V3 temporal v3_posicionobjetivo en la misma posición del target y le añade el offset de posición...
		v3_PosicionObjetivo = transform_objetivo.position + PS_positionSetting.targetPosOffset;

		//TODO; borrar esta linea
		//v3_destination = -Vector3.forward * PS_positionSetting.distanceFromTarget;
		v3_destination	 = 
			Quaternion.Euler (OS_orbitSetting.xRotation, OS_orbitSetting.yRotation, 0) // + transform_objetivo.eulerAngles.y, 0) 				
				
				* -Vector3.forward * PS_positionSetting.distanceFromTarget;

		v3_destination += v3_PosicionObjetivo;


		if (CH_collision.colliding) {

			v3_adjustedDestination = 
				Quaternion.Euler (OS_orbitSetting.xRotation, OS_orbitSetting.yRotation + transform_objetivo.eulerAngles.y, 0) 
				* Vector3.forward 
				* PS_positionSetting.adjustmentDistance;
			
			v3_adjustedDestination += v3_PosicionObjetivo;

			if (PS_positionSetting.smoothFollow) {
			
				transform.position = Vector3.SmoothDamp (transform.position, v3_adjustedDestination, ref v3_camVel, PS_positionSetting.smooth);

			} else {
				transform.position = v3_adjustedDestination;
			}

		} else {
			if (PS_positionSetting.smoothFollow) {

				transform.position = Vector3.SmoothDamp (transform.position, v3_destination, ref v3_camVel, PS_positionSetting.smooth);

			} else {
				transform.position = v3_destination;
			}
		}
	}



	void LookAtTarget()
	{

		Quaternion targetRotation = Quaternion.LookRotation (v3_PosicionObjetivo - transform.position);

		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, PS_positionSetting.lookSmooth * Time.deltaTime);


	}

	void OrbitTarget(){

		if (f_hOrbitSnapInput > 0) {
			OS_orbitSetting.yRotation = -180;
		}

		OS_orbitSetting.xRotation += -f_vOrbitInput * OS_orbitSetting.vOrbitSmooth * Time.deltaTime;
		OS_orbitSetting.yRotation += -f_hOrbitInput * OS_orbitSetting.hOrbitSmooth * Time.deltaTime;

		if (OS_orbitSetting.xRotation > OS_orbitSetting.maxXRotation) {
			OS_orbitSetting.xRotation = OS_orbitSetting.maxXRotation;
		}

		if (OS_orbitSetting.xRotation < OS_orbitSetting.minXRotation) {
			OS_orbitSetting.xRotation = OS_orbitSetting.minXRotation;
		}

	}




	void ZoomInOnTarget()
	{
		PS_positionSetting.distanceFromTarget += f_zoomInput * PS_positionSetting.zoomSmooth * Time.deltaTime;

		if (PS_positionSetting.distanceFromTarget > PS_positionSetting.maxZoom) {
			PS_positionSetting.distanceFromTarget = PS_positionSetting.maxZoom;
		}

		if (PS_positionSetting.distanceFromTarget < PS_positionSetting.minZoom) {
			PS_positionSetting.distanceFromTarget = PS_positionSetting.minZoom;
		}
	}

	void UpdateCollisionPoints(){


		CH_collision.UpdateCameraClipPoints (transform.position, transform.rotation, ref CH_collision.adjustedCameraClipPoints);
		CH_collision.UpdateCameraClipPoints(v3_destination, transform.rotation, ref CH_collision.desiredCameraClipPoints);

		for(int i = 0; i < 5; i++ )
		{
			if (DS_debugSetting.drawDesiredCollisionLines)
				Debug.DrawLine (v3_PosicionObjetivo, CH_collision.desiredCameraClipPoints [i], Color.white);
			if (DS_debugSetting.drawAdjustedCollisionLines)
				Debug.DrawLine (v3_PosicionObjetivo, CH_collision.adjustedCameraClipPoints [i], Color.green);

		}

		CH_collision.CheckColliding (v3_PosicionObjetivo);

		PS_positionSetting.adjustmentDistance = CH_collision.GetAdjustedDistanceWithRayFrom (v3_PosicionObjetivo);

	}

	[System.Serializable]
	public class CollisionHandler
	{
		public LayerMask collisionLayer;

		[HideInInspector]
		public bool colliding = false;

		[HideInInspector]
		public Vector3[] adjustedCameraClipPoints;

		[HideInInspector]
		public Vector3[] desiredCameraClipPoints;


		Camera camera;

		public void Initialize(Camera cam)
		{
			camera = cam;

			adjustedCameraClipPoints = new Vector3[5];
			desiredCameraClipPoints = new Vector3[5];

		}

		public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
		{

			if (!camera)
				return;

			//clear the contents of intoArray
			intoArray = new Vector3[5];

			float z = camera.nearClipPlane;
			float x = Mathf.Tan (camera.fieldOfView / 3.41f) * z;
			float y = x / camera.aspect;

			//top left
			intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; //addd and rotated the point relative to camera

			//top right
			intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition; //addd and rotated the point relative to camera

			//bottom left
			intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition; //addd and rotated the point relative to camera
		
			//bottom right
			intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition; //addd and rotated the point relative to camera

			//camera's position
			intoArray [4] = cameraPosition - camera.transform.forward;



		}

		bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
		{

			for (int i = 0; i < clipPoints.Length; i++) {
			
				Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
				float distance = Vector3.Distance(clipPoints[i] , fromPosition);

				if (Physics.Raycast (ray, distance, collisionLayer)) 
				{
					return true;
				}
			}

			return false;

		}



		public float GetAdjustedDistanceWithRayFrom(Vector3 from)
		{
			float distance = -1;

			for (int i = 0; i < desiredCameraClipPoints.Length; i++) 
			{
				Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit)) {
					if (distance == -1) {
						distance = hit.distance;
					} else {
						if (hit.distance < distance) {
							distance = hit.distance;
						}
					}
				}
			}

			if (distance == -1)
				return 0;
			else
				return  distance;

		}

		public void CheckColliding(Vector3 targetPosition)
		{
			if (CollisionDetectedAtClipPoints (desiredCameraClipPoints, targetPosition)) {
				colliding = true;
			} else {
				colliding = false;
			}
		}
	}

	[System.Serializable]
	public class PositionSettings
	{

		public Vector3 targetPosOffset = new Vector3(5,3.4f,0);
		public float lookSmooth = 0.09f;
		public float distanceFromTarget = -8;

		public float zoomSmooth = 10;
		public float maxZoom = -2;
		public float minZoom = -15;
		public bool smoothFollow = true;
		public float smooth = 0.05f;

		[HideInInspector]
		public float newDistance = -8;

		[HideInInspector]
		public float adjustmentDistance = -8;
	}

	[System.Serializable]
	public class OrbitSettings
	{

		public float xRotation = -20;
		public float yRotation = -180;
		public float maxXRotation = 25;
		public float minXRotation = -85;
		public float vOrbitSmooth = 150;
		public float hOrbitSmooth = 150;

	}

	[System.Serializable]
	public class InputSettings
	{
		public string ORBIT_HORIZONTAL_SNAP = "R3_Button";
		public string ORBIT_HORIZONTAL = "RStick_LeftRight";
		public string ORBIT_VERTICAL = "RStick_UpDown";
		public string ZOOM = "Mouse ScrollWheel";
	}

	[System.Serializable]
	public class DebugSettings
	{
		public bool drawDesiredCollisionLines = true;
		public bool drawAdjustedCollisionLines = true;
	}
}

