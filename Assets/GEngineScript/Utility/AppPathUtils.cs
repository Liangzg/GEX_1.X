/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 描述：应用路径工具集
/// <para>创建时间：2016-06-25</para>
/// </summary>
public sealed class AppPathUtils
{
    public const string AssetBundleMap = "00000016c800000000e8d5d000000000.txt";
    public const string AssetForMap = "AssetForBundle:";
    public const string BundleSuffix = ".assetsbundle";
    /// <summary>
    /// 使用WWW加载时，使用的持久化目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string WWWPathFormat(string path)
    {
#if UNITY_EDITOR
        return "file:///" + PersistentDataRootPath + path;
#elif UNITY_ANDROID || UNITY_IPHONE
        return "file://" + PersistentDataRootPath + path;
#endif
    }

    /// <summary>
    /// 使用WWW加载时，使用的StreamingAsset目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string WWWStreamingPathFormat(string path)
    {
#if UNITY_EDITOR
        return string.Concat("file:///" , Application.streamingAssetsPath , "/" , path);
#elif UNITY_ANDROID 
        return string.Concat("jar:file://" , Application.streamingAssetsPath , "/" , path);
#elif UNITY_IPHONE
        return string.Concat("file://" , Application.streamingAssetsPath  , "/" , path);
#endif        
    }

    /// <summary>
    /// 外部持久化根目录
    /// </summary>
    public static string PersistentDataRootPath
    {
        get
        {
#if UNITY_EDITOR
            string pathAll = Path.Combine(Application.dataPath, "../../AppResBin/");
            pathAll = Path.GetFullPath(pathAll);
            return pathAll;
#elif UNITY_ANDROID || UNITY_IPHONE
            return Application.persistentDataPath;
#endif
        }
    }

    
    /// <summary>
    /// 搜索文件是否存在，优先搜索外部目录
    /// </summary>
    /// <param name="path"></param>
    /// <param name= "isWWW">是否使用WWW加载</param>
    /// <returns>默认返回沙盒目录</returns>
    public static string IsSeachExist(string path , bool isWWW)
    {
        string persistendPath = Path.Combine(PersistentDataRootPath, path);
        if(File.Exists(persistendPath))
            return isWWW ? WWWPathFormat(path) : persistendPath;
        return isWWW ? WWWStreamingPathFormat(path) : string.Concat(Application.streamingAssetsPath, "/", path);
    }


}
