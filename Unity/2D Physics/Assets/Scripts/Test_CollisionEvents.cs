using UnityEngine;
using System.Collections;

public class Test_CollisionEvents : MonoBehaviour
{
	int fixedFrameCount = 0;
	void Start()
	{
		Debug.Log("Start");
		Physics2D.gravity = Vector2.zero;
	}

	void FixedUpdate()
	{
		fixedFrameCount++;
		Debug.Log(fixedFrameCount + "--------------------------------------------------");
	}
}
