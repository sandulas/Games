using UnityEngine;
using System.Collections;

public class MyInputEvents : MonoBehaviour
{
	public event SingleTouchHandler OnTouch, OnDrag, OnTap, OnRelease;

	public void Touch(GameObject sender, Camera camera)
	{
		if (OnTouch != null) OnTouch(sender, camera);
	}

	public void Drag(GameObject sender, Camera camera)
	{
		if (OnDrag != null) OnDrag(sender, camera);
	}

	public void Tap(GameObject sender, Camera camera)
	{
		if (OnTap != null) OnTap(sender, camera);
	}

	public void Release(GameObject sender, Camera camera)
	{
		if (OnRelease != null) OnRelease(sender, camera);
	}


	public delegate void SingleTouchHandler(GameObject sender, Camera camera);
}
