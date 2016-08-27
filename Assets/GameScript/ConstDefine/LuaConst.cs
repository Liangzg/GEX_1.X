using System.IO;
using LuaInterface;
using UnityEngine;

public static class LuaConst
{
    //lua逻辑代码目录
    public static string[] luaDirs =
    {
        Application.dataPath + "/GEngineScript/Lua" ,
        Application.dataPath + "/GameScript/Lua"
    }; 
    public static string toluaDir = Application.dataPath + "/Libs/ToLua/Lua";        //tolua lua文件目录

#if UNITY_STANDALONE
    public static string osDir = "Runtime";
#elif UNITY_ANDROID
    public static string osDir = "Android";            
#elif UNITY_IPHONE
    public static string osDir = "iOS";        
#else
    public static string osDir = "";        
#endif

    public static string luaResDir = string.Format("{0}/{1}/Lua", AppPathUtils.PersistentDataRootPath, osDir);      //手机运行时lua文件下载目录    

#if UNITY_EDITOR_WIN || NITY_STANDALONE_WIN    
    public static string zbsDir = "D:/ZeroBraneStudio/lualibs/mobdebug";        //ZeroBraneStudio目录       
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	public static string zbsDir = "/Applications/ZeroBraneStudio.app/Contents/ZeroBraneStudio/lualibs/mobdebug";
#else
    public static string zbsDir = luaResDir + "/mobdebug/";
#endif    

    public static bool openLuaSocket = true;            //是否打开Lua Socket库
    public static bool openZbsDebugger = false;         //是否连接ZeroBraneStudio调试

    /// <summary>
    /// 添加Lua文件搜索目录
    /// </summary>
    public static void AddSearchDir()
    {
        if (!Directory.Exists(LuaConst.toluaDir))
        {
            string msg = string.Format("toluaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.toluaDir);
            throw new LuaException(msg);
        }
        LuaState.AddSearchPathStatic(LuaConst.toluaDir);

        //添加项目Lua文件路径
        foreach (string luaDir in luaDirs)
        {
            addAllDir(luaDir);
        }
    }

    /// <summary>
    /// 添加所有子目录路径
    /// </summary>
    /// <param name="rootDir"></param>
    private static void addAllDir(string rootDir)
    {
        if (!Directory.Exists(rootDir))
        {
            string msg = string.Format("luaDir path not exists: {0}, configer it in LuaConst.cs", rootDir);
            throw new LuaException(msg);
        }

        string[] subDirs = Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories);

        foreach (string subDir in subDirs)
        {
            string dir = subDir.Replace("\\", "/");
            LuaState.AddSearchPathStatic(dir);
        }

        LuaState.AddSearchPathStatic(rootDir);
    }
}