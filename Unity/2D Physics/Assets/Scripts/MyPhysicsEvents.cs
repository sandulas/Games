using UnityEngine;
using System.Collections;

public class MyPhysicsEvents : MonoBehaviour
{
	public delegate void CollisionEnterHandler(Collision2D collision);

	public event CollisionEnterHandler OnCollisionEnter;

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (OnCollisionEnter != null) OnCollisionEnter(collision);
	}
}
