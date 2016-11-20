using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class myCharacterController : MonoBehaviour {


	[System.Serializable]
	public class MoveSettings
	{
		
		public float forwardVel = 12;
		public float rotateVel = 100;
		public float jumpVel = 25;
		public float distToGrounded = 0.1f;

		[HideInInspector]
		public Camera m_Cam;

		[HideInInspector]
		public Vector3 m_CamForward;
		[HideInInspector]
		public float m_TurnAmount;

		public LayerMask ground;

		public void SetCamera(Camera cam)
		{
			m_Cam = cam;
		}
	}

	[System.Serializable]
	public class PhysicsSettings
	{
		public float downAccel = 0.75f;
	}

	[System.Serializable]
	public class InputSettings
	{
		public float inputDelay = 0.1f;
		public string forward_axis = "LStick_UpDown";
		public string turn_axis = "LStick_LeftRight";
		public string jump_axis = "Square";
	}

	[System.Serializable]
	public class AnimatorSettings
	{

		public string float_speed = "speed";
		public string float_jump = "jump";
		public string float_turn = "turn";
		public string trigger_death = "jump";
		public string bool_death = "OnGround";

		Animator animatorInstance;

		public void SetAnimator(Animator animator){
			this.animatorInstance = animator;
		}

		public void SetAnimation(string ParameterName, float value)
		{

			animatorInstance.SetFloat (ParameterName, value);
		}
	}


	public MoveSettings move = new MoveSettings();
	public PhysicsSettings physics = new PhysicsSettings ();
	public InputSettings input = new InputSettings();
	public AnimatorSettings characteranimation = new AnimatorSettings ();

	Vector3 velocity;

	Quaternion targetRotation;
	Rigidbody rBody;
	float forwardInput, turnInput, jumpInput;

	public Quaternion TargetRotation{
		get { return targetRotation; }
	}

	bool Grounded()
	{

		Debug.DrawRay (transform.position + Vector3.up * 0.1f, Vector3.down * move.distToGrounded, Color.blue);

		return Physics.Raycast (transform.position + Vector3.up * 0.1f, Vector3.down, move.distToGrounded, move.ground);
	}

	void Start()
	{
		targetRotation = transform.rotation;

		if (GetComponent<Rigidbody> ())
			rBody = GetComponent<Rigidbody> ();
		else
			Debug.Log ("Character needs rigidbody");


		if (GetComponent<Animator> ())
			characteranimation.SetAnimator (GetComponent<Animator> ());
		else
			Debug.Log ("Character missing animator");

		if (Camera.main)
			move.SetCamera (Camera.main);
		else
			Debug.Log ("Scene is missing a main camera");

	
		forwardInput = turnInput = jumpInput = 0;
	
	}

	void GetInput()
	{

		forwardInput = CrossPlatformInputManager.GetAxis (input.forward_axis);
		turnInput = CrossPlatformInputManager.GetAxis (input.turn_axis);
		jumpInput = CrossPlatformInputManager.GetAxisRaw (input.jump_axis);

	}

	void Update()
	{
		GetInput ();
		Turn ();
	}

	void FixedUpdate()
	{
		Run ();
		Jump ();

		rBody.velocity = velocity;



	}

	void Run()
	{
		if (Mathf.Abs (forwardInput) > input.inputDelay && Grounded()) {
		

			velocity.z = move.forwardVel * forwardInput;


		} else if(Grounded()) {
		
			velocity = Vector3.zero;
		}

		characteranimation.SetAnimation (characteranimation.float_speed, forwardInput);
	}

	void Turn()
	{
		if (Mathf.Abs (turnInput) > input.inputDelay) {
			targetRotation *= Quaternion.AngleAxis (move.rotateVel * turnInput * Time.deltaTime, Vector3.up);
			transform.rotation = targetRotation;
		}

		characteranimation.SetAnimation (characteranimation.float_turn, turnInput);
	}



	void Jump()
	{
		if (jumpInput > 0 && Grounded ()) {
			velocity.y = move.jumpVel;
		} else if (jumpInput == 0 && Grounded ()) {
			velocity.y = 0;
		} else {
			velocity.y -= physics.downAccel;
		}
	}




}
