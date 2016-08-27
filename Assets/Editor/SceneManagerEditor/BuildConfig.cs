/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngineInternal;

/// <summary>
/// 资源工具所需的全局路径
/// </summary>
public class BuildGlobal
{
    /// <summary>
    /// 保存导出文件的根目录
    /// </summary>
    public static string EXPORT_BUNDLE_ROOT
    {
        get
        {
            string path = Path.Combine(Application.dataPath, "../../export/");
            path = Path.GetFullPath(path);
            return path;
        }
    }

    /// <summary>
    /// 配置保存目录
    /// </summary>
    public static string EXPORT_CONFIG_DIR = EXPORT_BUNDLE_ROOT + "Config/";

    public const string Meta = ".meta";

    public static HashSet<string> FilterAsset = new HashSet<string>()
    {
        ".cs" , ".shader"
    };

    //全局索引文件，只映射指定类型的文件
    public static HashSet<string> MapIndexFilter = new HashSet<string>(new[] { ".prefab", ".txt" });
}

/// <summary>
/// 描述：打包配置
/// <para>创建时间：</para>
/// </summary>
[Serializable]
public class BuildConfig
{
    /// <summary>
    /// 自定义的Bundle名称(可选)
    /// </summary>
    [SerializeField] 
    public string BundleName = "";
    /// <summary>
    /// 输入路径
    /// </summary>
    [SerializeField]
    public string InputDir="";
    /// <summary>
    /// 输出路径
    /// </summary>
    [SerializeField]
    public string OutputDir="";
    /// <summary>
    /// 打包策略
    /// </summary>
    [SerializeField]
    public string BuildStrategy;
    /// <summary>
    /// 文件查找方式
    /// </summary>
    [SerializeField]
    public SearchOption OptionSerach;
    /// <summary>
    /// 前置打包
    /// </summary>
    [SerializeField]
    public string PreBuild;

    /// <summary>
    /// 文件后缀
    /// </summary>
    [SerializeField]
    public string FileSuffixs = "*.prefab";

    /// <summary>
    /// 资源的分类
    /// </summary>
    [SerializeField]
    public string AssetType = "None";

}


public class BuildConfigManager : ASignalEntry<BuildConfigManager>
{
    private Dictionary<string , BuildConfig> configDic = new Dictionary<string, BuildConfig>(); 

    private string allPath = Path.Combine(BuildGlobal.EXPORT_CONFIG_DIR, "buildConfigs.json");


    private BuildConfigManager() { }

    public void ReadConfig()
    {
        configDic.Clear();
        if (!File.Exists(allPath)) return;

        string configJson = File.ReadAllText(allPath);
        
        BuildConfig[] configs = AssetBundleUtil.FromJsonArray<BuildConfig>(ref configJson);
        foreach (BuildConfig buildConfig in configs)
        {
            AddBuildConfig(buildConfig);
        }
    }

    public void SaveConfig() { 
        string jsonStr = AssetBundleUtil.ToJsonArray(BuildConfigs);
        if (string.IsNullOrEmpty(jsonStr)) return;

        string dir = Path.GetDirectoryName(allPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(allPath , jsonStr);
    }

    public BuildConfig GetBuildConfig(string key)
    {
        if (!configDic.ContainsKey(key)) return null;

        return configDic[key];
    }


    public BuildConfig[] BuildConfigs
    {
        get
        {
            BuildConfig[] configs = new BuildConfig[configDic.Count];
            configDic.Values.CopyTo(configs , 0);
            return configs;
        }
    }


    public BuildConfig AddBuildConfig(string configName)
    {
        if (configDic.ContainsKey(configName)) return configDic[configName];

        BuildConfig bc = new BuildConfig();
        bc.BundleName = configName;

        configDic[configName] = bc;
        return bc;
    }

    public BuildConfig AddBuildConfig(BuildConfig bc)
    {
        if (configDic.ContainsKey(bc.BundleName)) return configDic[bc.BundleName];
        
        configDic[bc.BundleName] = bc;
        return bc;
    }
    /// <summary>
    /// 根据资源的项目相对路径查找对应的配置信息
    /// </summary>
    /// <param name="assetPath">相对项目的路径 eg:Assets/Res/xxx.prefab</param>
    /// <returns></returns>
    public BuildConfig FindAssetBuildConfig(string assetPath)
    {
        string assetRootPath = assetPath.Substring(0 , assetPath.LastIndexOf("/"));
        assetRootPath = assetRootPath.Replace("Assets/", "");

        //查找最佳匹配
        int length = 0;
        BuildConfig config = null;
        foreach (BuildConfig buildConf in configDic.Values)
        {
            if (assetRootPath.StartsWith(buildConf.InputDir) && buildConf.InputDir.Length > length)
            {
                length = buildConf.InputDir.Length;
                config =  buildConf;
            }
        }
        return config;
    }

    public void RemoveConfig(string configName)
    {
        configDic.Remove(configName);
    }
}



public sealed class AssetBundleUtil
{
    /// <summary>
    /// 分配Bundle管理
    /// </summary>
    /// <param name="assetPath">资源的项目相对路径 eg:Assets/Res/xxx.unity</param>
    /// <param name="bundleName">AssetBundle的名称 eg:Res01/res</param>
    /// <param name="variant"></param>
    public static void AssignBundle(string assetPath, string bundleName, string variant)
    {
        AssetImporter assetImpt = AssetImporter.GetAtPath(assetPath);
        if (assetImpt == null) return;

        assetImpt.assetBundleName = bundleName;
        assetImpt.assetBundleVariant = variant;
    }


    /// <summary>
    /// 分配Bundle管理
    /// </summary>
    /// <param name="assetPath">资源的项目相对路径 eg:Assets/Res/xxx.unity</param>
    /// <param name="bundleName">AssetBundle的名称 eg:Res01/res</param>
    public static void AssignBundle(string assetPath, string bundleName)
    {
        if (assetPath.IndexOf(":") != -1)
            assetPath = assetPath.Replace(Application.dataPath, "Assets");

        string newBundleName = bundleName;
        if (!bundleName.EndsWith(".assetsbundle"))
            newBundleName += ".assetsbundle";
        AssignBundle(assetPath , newBundleName, null);
    }
    /// <summary>
    /// 清除Bundle绑定
    /// </summary>
    /// <param name="assetPath">资源的项目相对路径 eg:Assets/Res/xxx.unity</param>
    public static void ClearBundle(string assetPath)
    {
        if (assetPath.IndexOf(":") != -1)
            assetPath = assetPath.Replace(Application.dataPath, "Assets");
        AssignBundle(assetPath , null, null);
    }

    public static string FormatOutputPath(BuildConfig buildConf)
    {
        return FormatOutputPath(buildConf.OutputDir);
    }

    /// <summary>
    /// 格式化输出路径
    /// </summary>
    /// <param name="outputPath">相对路径格式 eg:../../xxxx/</param>
    /// <returns>完整的绝对输出路径</returns>
    public static string FormatOutputPath(string outputPath)
    {
        string allPath = Path.Combine(BuildGlobal.EXPORT_BUNDLE_ROOT, outputPath);
        allPath = Path.GetFullPath(allPath);
        return allPath;
    }

    /// <summary>
    /// 同一目录，多个文件打包到同一个Bundle内
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="assetType"></param>
    /// <param name="absolutionPath"></param>
    /// <param name="findFile"></param>
    /// <param name="optionSearch"></param>
    public static void MulInOneBundle(string bundleName, string assetType,
                                        string absolutionPath, string[] findFile, SearchOption optionSearch)
    {
        List<string> files = new List<string>();
        foreach (string fileSuffix in findFile)
        {
            string[] fileArr = Directory.GetFiles(absolutionPath, fileSuffix, optionSearch);
            files.AddRange(fileArr);
        }

        //为文件分配到指定Bundle
        List<string> allBundleFile = new List<string>();
        foreach (string filePath in files)
        {
            if (filePath.EndsWith(BuildGlobal.Meta)) continue;

            string relativePath = filePath.Replace("\\", "/");
            relativePath = relativePath.Replace(Application.dataPath, "Assets");

            List<string> bundleFiles = AssetBuildEditor.Instance.AssignBundle(bundleName, relativePath, bundleName);
            allBundleFile.AddRange(bundleFiles);
        }
    }

    #region ----------------JSON 序列化--------------------------------------
    public static T[] FromJsonArray<T>(ref string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.array;
    }


    public static string ToJsonArray<T>(T[] array)
    {
        if (array == null || array.Length <= 0) return null;

        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;

        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
    #endregion


}