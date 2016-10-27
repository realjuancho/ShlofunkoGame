using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider))]
public class TouchButtons : MonoBehaviour {

	// Use this for initialization

	public bool isTouchedDown;
	public bool isTouched;

	public string Name;

	void Start()
	{


	}

	void Update()
	{

		if(Input.touches.Length > 0)
		{


		}

	}

	void OnTriggerEnter(Collider col)
	{

	}

	void OnTriggerStay(Collider col)
	{
		
	}
}
