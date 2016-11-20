using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{

		// The target we are following
		[SerializeField]
		private Transform target;
		// The distance in the x-z plane to the target
		[SerializeField]
		private float distance = 10.0f;
		// the height we want the camera to be above the target
		[SerializeField]
		private float height = 5.0f;

		[SerializeField]
		private float rotationDamping = 0.5f;
		[SerializeField]
		private float heightDamping = 0.5f;

		// Use this for initialization
		void Start() { }

		void Update(){

			GetCameraTurnInput ();

		}


		//float wantedRotationAngle;
		float lastRotationAngle;

		// Update is called once per frame
		void LateUpdate()
		{
			// Early out if we don't have a target
			if (!target)
				return;

			// Calculate the current rotation angles
			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;


			if(rotationX != 0) wantedRotationAngle += lastRotationAngle * rotationY * Time.deltaTime * rotationVel; 
			//this.wantedRotationAngle = wantedRotationAngle;



			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y;

			lastRotationAngle = currentRotationAngle;


			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;


			// Set the height of the camera
			transform.position = new Vector3(transform.position.x ,currentHeight , transform.position.z);



			// Always look at the target
			transform.LookAt(target);
		}


		float rotationY;
		float rotationX;
		public float rotationVel = 10;

 

		void GetCameraTurnInput()
		{
			rotationY = CrossPlatformInputManager.GetAxis ("RStick_LeftRight");
			rotationX = CrossPlatformInputManager.GetAxis ("RStick_UpDown");
		}
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
}