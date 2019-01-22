/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.IO;
using LuaFramework;
using LuaInterface;

/// <summary>
/// 描述：用于测试Lua脚本
/// <para>创建时间：2016-08-04</para>
/// </summary>
public class TestLuaManager : MonoBehaviour
{

    public string LuaPath;

    public string Function;

    private BaseLuaManager mLuaMgr;

    private LuaFunction onGUIFunc;
    private string tableName;
    void Awake()
    {
        mLuaMgr = AppFacade.Instance.AddManager<BaseLuaManager>();
    }

    // Use this for initialization
    void Start()
    {
        mLuaMgr.InitStart();
        if (!string.IsNullOrEmpty(LuaPath))
        {
            if (!LuaPath.EndsWith(".lua")) LuaPath += ".lua";
            mLuaMgr.DoFile(LuaPath);
            tableName = Path.GetFileNameWithoutExtension(LuaPath);

            mLuaMgr.LuaMachine[tableName + ".Test"] = this;
        }

        callFunction(tableName, "Awake");
        
        onGUIFunc = getFunction(tableName, "OnGUI");

        callFunction(tableName, "Start");
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnGUI()
    {

        if (!string.IsNullOrEmpty(Function) && GUILayout.Button("Call Function"))
        {
            mLuaMgr.CallFunction(Function);
        }

        if (onGUIFunc != null)
            onGUIFunc.Call();
    }


    private void OnDestroy()
    {
        callFunction(tableName, "OnDestroy");
    }

    private LuaFunction getFunction(string tableName, string funcName)
    {
        LuaFunction func = mLuaMgr.GetFunction(tableName + "." + funcName);
        if (func == null)
            func = mLuaMgr.GetFunction(tableName + ":" + funcName);
        return func;
    }


    private void callFunction(string tableName, string funcName)
    {
        LuaFunction func = getFunction(tableName, funcName);
        if (func == null) return;

        func.Call();
    }
}



