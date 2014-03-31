using UnityEngine;
using System.Collections;

public class AddForce_GameObject : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void FixedUpdate()
	{
		gameObject.rigidbody2D.AddForce(new Vector2(0, 170));
	}
}
