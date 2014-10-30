using UnityEngine;
using System.Collections;

public class MyPhysicsEvents : MonoBehaviour
{
	public delegate void TriggerHandler(Collider2D otherCollider);

	public event TriggerHandler OnTriggerEnter, OnTriggerStay, OnTriggerExit;

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (OnTriggerEnter != null) OnTriggerEnter(otherCollider);
	}
	void OnTriggerStay2D(Collider2D otherCollider)
	{
		if (OnTriggerStay != null) OnTriggerStay(otherCollider);
	}
	void OnTriggerExit2D(Collider2D otherCollider)
	{
		if (OnTriggerExit != null) OnTriggerExit(otherCollider);
	}
}
