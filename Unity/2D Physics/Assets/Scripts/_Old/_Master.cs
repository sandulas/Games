using UnityEngine;
using System.Collections;

public class HingeJoint_Master : MonoBehaviour
{
	public GameObject object1;
	public GameObject object2;

	// Use this for initialization
	void Start()
	{
		HingeJoint2D joint = object1.AddComponent<HingeJoint2D>();
		joint.connectedBody = object2.rigidbody2D;
		
		joint.collideConnected = false;

		JointAngleLimits2D limits = new JointAngleLimits2D();
		limits.min = -10;
		limits.max = 0;
		joint.limits = limits;
		joint.useLimits = true;

		joint.anchor = new Vector2(0, 0);
		joint.connectedAnchor = new Vector2(0, -2);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
