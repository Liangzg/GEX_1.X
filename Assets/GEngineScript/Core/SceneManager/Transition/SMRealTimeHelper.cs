// (C) 2013 Ancient Light Studios. All rights reserved.
// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
/// <summary>
/// Class which allows to get a time-scale independent deltaTime.
/// 
/// </summary>
using UnityEngine;


public class SMRealTimeHelper : MonoBehaviour
{
	private float lastUpdate;
	private float lastDelta;
	private static SMRealTimeHelper instance;

	private SMRealTimeHelper ()
	{
	}

	private static SMRealTimeHelper Instance {
		get {
			if (instance == null) {
				GameObject go = new GameObject ("_SceneManagerRealTimeHelper");
				go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
				DontDestroyOnLoad (go);
				instance = go.AddComponent<SMRealTimeHelper> ();
				instance.lastUpdate = Time.realtimeSinceStartup;
			}
			return instance;
		}
	}

	public static float deltaTime {
		get {
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				return 0f;
			}
#endif
			return Instance.lastDelta;
		}
	}

	public void Update() {
		var now = Time.realtimeSinceStartup;
		lastDelta = now - lastUpdate;
		lastUpdate = now;
	}

}
