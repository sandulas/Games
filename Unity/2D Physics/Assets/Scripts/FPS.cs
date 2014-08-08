﻿using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{

	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.

	public float updateInterval = 0.5F;

	private float accum = 0; // FPS accumulated over the interval
	private int frames = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	float fps;

	void Start()
	{
		timeleft = updateInterval;
		//QualitySettings.antiAliasing = 0;
		Application.targetFrameRate = 60;
	}

	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;

		// Interval ended - update GUI text and start new interval
		if (timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
			fps = accum / frames;

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}

	void OnGUI()
	{
		GUI.contentColor = new Color(1, 0, 0, 1);
		GUI.skin.label.fontSize = 16;
		GUI.Label(new Rect(5, Screen.height - 25, 100, 100), System.String.Format("{0:F2} FPS", fps));
	}
}