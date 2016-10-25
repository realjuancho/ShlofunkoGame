using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {


	Animator chestAnimatorController;

	// Use this for initialization
	void Awake () {

		chestAnimatorController = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void LateUpdate () {


		if(playerInRange)
		{
			if(player)
			{
				if(player.actionContext == Player.ActionContext.Open && player.triggerAction)
					OpenChest();
			}
		}

	}

	[SerializeField]
	bool playerInRange;
	Player player;

	void OnTriggerEnter(Collider col)
	{
		player = col.gameObject.GetComponent<Player>();
		if(player)
		{
			Debug.Log("Player In Range");
			player.SetActionContext(Player.ActionContext.Open);
			playerInRange = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		player = col.gameObject.GetComponent<Player>();
		if(player)
		{

			Debug.Log("Player out of Range");
			player.SetActionContext(Player.ActionContext.Null);
			playerInRange = false;

		}
	}


	void OpenChest()
	{

		chestAnimatorController.SetTrigger("OpenChest");

	}
}
