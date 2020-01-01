using System.Collections;
using System.Collections.Generic;
using GEX.Resource;
using UnityEngine;

public class TestAssetLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUILayout.Label("Parllerl Or Serial Loader:");

        if (GUILayout.Button("SerialLoader"))
        {
            TestSerialLoader();
        }

        if (GUILayout.Button("ParllerlLoader"))
        {
            TestParallelLoader();
        }
    }


    private void TestSerialLoader()
    {
        SerialLoader loader = new SerialLoader();

        loader.AddLoader("Models/GO1.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(0.5f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO2.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(1.0f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO3.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(1.5f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO4.prefab").OnFinish = InstanceGameObject;

        this.StartCoroutine(loader);
    }


    private void TestParallelLoader()
    {
        ParallelLoader loader = new ParallelLoader();

        loader.AddLoader("Models/GO1.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(0.5f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO2.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(1.0f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO3.prefab").OnFinish = InstanceGameObject;
        loader.AddLoader(new LoadDelayAsync(1.5f)).OnFinish = PrintTime;
        loader.AddLoader("Models/GO4.prefab").OnFinish = InstanceGameObject;

        this.StartCoroutine(loader);
    }


    private void InstanceGameObject(LoadOperation loadOpt)
    {
        GameObject.Instantiate(loadOpt.GetAsset<GameObject>());
    }

    private void PrintTime(LoadOperation loadOpt)
    {
        Debug.Log("Time:" + Time.realtimeSinceStartup);
    }
}
