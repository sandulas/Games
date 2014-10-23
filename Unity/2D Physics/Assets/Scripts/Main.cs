using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
	GameObject buttonMenu;

	void Start()
	{
		MyInputEvents inputEvents;

		buttonMenu = GameObject.Find("ButtonMenu");
		inputEvents = buttonMenu.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonMenu_OnTouch;
		inputEvents.OnDrag += ButtonMenu_OnDrag;
		inputEvents.OnTap += ButtonMenu_OnTap;
		inputEvents.OnRelease += ButtonMenu_OnRelease;
	}

	void Update()
	{
	}


	private void ButtonMenu_OnTouch(GameObject sender, Camera camera)
	{
		Debug.Log("Touch: " + sender.name + " -> " + camera.name);
	}

	private void ButtonMenu_OnDrag(GameObject sender, Camera camera)
	{
		Debug.Log("Drag: " + sender.name + " -> " + camera.name);
	}

	private void ButtonMenu_OnTap(GameObject sender, Camera camera)
	{
		Debug.Log("Tap: " + sender.name + " -> " + camera.name);
	}

	private void ButtonMenu_OnRelease(GameObject sender, Camera camera)
	{
		Debug.Log("Release: " + sender.name + " -> " + camera.name);
	}
}
