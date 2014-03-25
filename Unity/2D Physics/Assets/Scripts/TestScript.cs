using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		//Vector3 position = gameObject.transform.localPosition;

		//if (position.y > 0) return;

		//position.y += 0.1f;
		//gameObject.transform.localPosition = position;
	}

	void FixedUpdate()
	{
		gameObject.rigidbody2D.AddForce(new Vector2(0, 150));
		gameObject.rigidbody2D.AddForce(new Vector2(0, 150));

	}
}
