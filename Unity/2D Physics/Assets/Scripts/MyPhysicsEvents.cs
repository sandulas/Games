using UnityEngine;
using System.Collections;

public class MyPhysicsEvents : MonoBehaviour
{
	public delegate void TriggerHandler(Collider2D otherCollider);

	public event TriggerHandler OnTriggerEnter, OnTriggerStay, OnTriggerExit;

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		//Debug.Log("Trigger Enter - " + gameObject.name);
		if (OnTriggerEnter != null) OnTriggerEnter(otherCollider);
	}
	void OnTriggerStay2D(Collider2D otherCollider)
	{
		//Debug.Log("Trigger Stay - " + gameObject.name);
		if (OnTriggerStay != null) OnTriggerStay(otherCollider);
	}
	void OnTriggerExit2D(Collider2D otherCollider)
	{
		//Debug.Log("Trigger Exit - " + gameObject.name);
		if (OnTriggerExit != null) OnTriggerExit(otherCollider);
	}
}
