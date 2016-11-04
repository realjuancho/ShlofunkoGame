using UnityEngine;
using System.Collections;

public class CoinPickup : MonoBehaviour {


	Animator anim;

	void Start()
	{
		anim = GetComponent<Animator>();
	}

	void OnTriggerEnter(Collider col)
	{

		if(col.gameObject.GetComponent<Player>())
		{

			anim.SetTrigger("pickup");
			

		}


	}
}
