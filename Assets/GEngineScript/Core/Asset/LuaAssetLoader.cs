/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using LuaFramework;
using LuaInterface;

/// <summary>
/// 描述：Lua层资源加载
/// <para>创建时间：2016-08-04</para>
/// </summary>
public sealed class LuaAssetLoader{
    
    /// <summary>
    /// 通过指定资源名加载GameObject资源
    /// </summary>
    /// <param name="resName">资源名称</param>
    /// <param name="luaCallback">加载实例完成后的回调</param>
    public static void LoadGameObjectByName(string resName, AssetLoader.EAssetType assetType, LuaFunction luaCallback)
    {
        Action<GameObject> callback = null;
        if(luaCallback != null)
            callback = (Action<GameObject>)DelegateFactory.CreateDelegate(typeof (Action<GameObject>), luaCallback);
        AssetLoader.LoadGameObjectByName(resName , assetType , callback);
    }

    public static void LoadGameObjectByName(string resName, AssetLoader.EAssetType assetType, string luaFuncName)
    {
        BaseLuaManager mgr = AppFacade.Instance.GetManager<BaseLuaManager>();
        LuaFunction luaCallback = mgr.GetFunction(luaFuncName);
        LoadGameObjectByName(resName , assetType , luaCallback);
    }

    /// <summary>
    /// 通过指定的UI资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">UI资源名</param>
    /// <param name="luaCallback">加载实例完成后的回调</param>
    public static void LoadUI(string resName, LuaFunction luaCallback)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.UI, luaCallback);
    }

    public static void LoadUI(string resName, string luaFuncName)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.UI, luaFuncName);
    }
    /// <summary>
    /// 通过指定的特效资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">特效资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadEffect(string resName, LuaFunction callback)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.Effect, callback);
    }

    public static void LoadEffect(string resName, string luaFuncName)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.Effect, luaFuncName);
    }
    /// <summary>
    /// 通过指定的模型资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">模型资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadModel(string resName, LuaFunction callback)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.Model, callback);
    }

    public static void LoadModel(string resName, string luaFuncName)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.Model, luaFuncName);
    }
    /// <summary>
    /// 加载一个普通的GameObject对象
    /// </summary>
    /// <param name="resName">模型资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadGameObject(string resName, LuaFunction callback)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.GameObject, callback);
    }

    public static void LoadGameObject(string resName, string luaFuncName)
    {
        LoadGameObjectByName(resName, AssetLoader.EAssetType.GameObject, luaFuncName);
    }


    /// <summary>
    /// 执行资源预加载
    /// </summary>
    /// <param name="luaCallback"></param>
    public static void OnInitPreload(LuaFunction luaCallback)
    {
        Action callback = null;
        if (luaCallback != null)
            callback = (Action)DelegateFactory.CreateDelegate(typeof(Action), luaCallback);
        AppInter.StartCoroutine(AssetLoader.Instance.InitPreLoad(callback));
    }
}
