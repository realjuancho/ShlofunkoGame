using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider))]
public class TouchButtons : MonoBehaviour {

	// Use this for initialization

	public string Name;
	public Material pushedButtonMaterial;


	MeshRenderer meshRenderer;
	Material originalMaterial;

	bool _isTouched;


	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer> ();

		originalMaterial = meshRenderer.material;

	}

	void Update()
	{
		CheckTouched ();
	}
		


	void CheckTouched()
	{
		if (_isTouched) {

			Debug.Log (Name);


			if (pushedButtonMaterial)
				meshRenderer.material = pushedButtonMaterial;


		} else 
		{
			meshRenderer.material = originalMaterial;
		}
		setTouched (false);
	}
		
	public void setTouched(bool isTouched)
	{
		_isTouched = isTouched;
	}

	public bool isTouched()
	{
		return _isTouched;
	}

}
