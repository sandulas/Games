using UnityEngine;
using System.Collections;

public class MyInputEvents : MonoBehaviour
{
	public bool allowScreenEvents = false;

	public delegate void SingleTouchHandler(GameObject sender, Camera camera);
	public delegate void SingleScreenTouchHandler();
	public delegate void DoubleScreenTouchHandler(Touch touch0, Touch touch1);
	public delegate void DoubleScreenTouchEndHandler();
	public delegate void MouseScrollWheelHandler(float amount);

	public event SingleTouchHandler OnTouch, OnDrag, OnTap, OnRelease;
	public event SingleScreenTouchHandler OnScreenTouch, OnScreenDrag, OnScreenRelease;
	public event DoubleScreenTouchHandler OnDoubleTouchStart, OnDoubleTouchDrag;
	public event DoubleScreenTouchEndHandler OnDoubleTouchEnd;
	public event MouseScrollWheelHandler OnMouseScrollWheel;

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

	public void ScreenTouch()
	{
		if (OnScreenTouch != null) OnScreenTouch();
	}
	public void ScreenDrag()
	{
		if (OnScreenDrag != null) OnScreenDrag();
	}
	public void ScreenRelease()
	{
		if (OnScreenRelease != null) OnScreenRelease();
	}

	public void ScreenDoubleTouchStart(Touch touch0, Touch touch1)
	{
		if (OnDoubleTouchStart != null) OnDoubleTouchStart(touch0, touch1);
	}
	public void ScreenDoubleTouchDrag(Touch touch0, Touch touch1)
	{
		if (OnDoubleTouchDrag != null) OnDoubleTouchDrag(touch0, touch1);
	}
	public void ScreenDoubleTouchEnd()
	{
		if (OnDoubleTouchEnd != null) OnDoubleTouchEnd();
	}

	public void MouseScrollWheel(float amount)
	{
		if (OnMouseScrollWheel != null) OnMouseScrollWheel(amount);
	}
}
