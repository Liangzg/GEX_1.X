using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#region ----------------常用非泛型委托------------------------

public delegate void VoidDelegate(GameObject gObj);

#endregion

public class AppConst {

#if UNITY_EDITOR
    public static bool DebugMode = true;                         //调试模式-用于内部测试,影响释放
    public static bool UpdateMode = false;                       //更新模式-默认关闭
    public static bool AssetBundleMode = false;                  //资源AssetBundle模式
    public static bool LuaBundleMode = false;                    //Lua代码AssetBundle模式-默认关闭
#else
    public static bool DebugMode = false;                        //调试模式-用于内部测试
    public static bool UpdateMode = true;                       //更新模式-默认开启(非编辑器)
    public static bool AssetBundleMode = true;                   //资源AssetBundle模式
    public static bool LuaBundleMode = true;                     //Lua代码AssetBundle模式-不是编辑器模式下默认打开
#endif

    public static bool BundleDebugMode = false;

    public static bool alwaysExtractData = false;

    public static bool LuaByteMode = false;                       //Lua字节码模式-默认关闭
    
    public const int TimerInterval = 1;
    public const int GameFrameRate = 30;                        //游戏帧频

    public const string AppName = "LuaFramework";               //应用程序名称
    public const string LuaTempDir = "Lua/";                    //临时目录
    public const string AppPrefix = AppName + "_";              //应用程序前缀
    public const string ExtName = ".unity3d";                   //素材扩展名
    public const string AssetDir = "StreamingAssets";           //素材目录 
    public const string WebUrl = "http://localhost:6688/";      //测试更新地址

    public static string UserId = string.Empty;                 //用户ID
    public static int SocketPort = 0;                           //Socket服务器端口
    public static string SocketAddress = string.Empty;          //Socket服务器地址

    public static string FrameworkRoot {
        get {
            return Application.dataPath + "/" + AppName;
        }
    }
}