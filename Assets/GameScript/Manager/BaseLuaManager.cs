/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using LuaFramework;
using LuaInterface;

/// <summary>
/// 描述：基础Lua管理器基类，保持测试管理器的部分接口统一
/// <para>创建时间：2016-08-05</para>
/// </summary>
public class BaseLuaManager : Manager
{
    protected LuaState lua;
    private LuaLooper loop = null;

    // Use this for initialization
    protected void Awake()
    {
        lua = new LuaState();
        this.OpenLibs();
        lua.LuaSetTop(0);

        LuaBinder.Bind(lua);
        LuaCoroutine.Register(lua, this);
    }

    public void InitStart()
    {
        this.lua.Start();    //启动LUAVM
        this.StartLooper();
    }

    void StartLooper()
    {
        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = lua;
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        lua.OpenLibs(LuaDLL.luaopen_cjson);
        lua.LuaSetField(-2, "cjson");

        lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
        lua.LuaSetField(-2, "cjson.safe");
    }


    /// <summary>
    /// 初始化加载第三方库
    /// </summary>
    void OpenLibs()
    {
        lua.OpenLibs(LuaDLL.luaopen_pb);
        lua.OpenLibs(LuaDLL.luaopen_lpeg);
        lua.OpenLibs(LuaDLL.luaopen_bit);
        lua.OpenLibs(LuaDLL.luaopen_socket_core);

        this.OpenCJson();
    }

    public object[] DoFile(string filename)
    {
        return lua.DoFile(filename);
    }

    /// <summary>
    /// Lua虚拟机
    /// </summary>
    public LuaState LuaMachine { get { return lua; } }

    // Update is called once per frame
    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            return func.Call(args);
        }
        return null;
    }

    /// <summary>
    /// 获得指定LuaFunction
    /// </summary>
    /// <param name="funcName"></param>
    /// <returns></returns>
    public LuaFunction GetFunction(string funcName)
    {
        return lua.GetFunction(funcName);
    }

    public void LuaGC()
    {
        lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    public void Close()
    {
        loop.Destroy();
        loop = null;

        lua.Dispose();
        lua = null;
    }
}
