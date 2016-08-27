/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LuaFramework;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 描述：资源打包编辑器逻辑
/// <para>创建时间：2016-06-20</para>
/// </summary>
public class AssetBuildEditor : EditorWindow
{
    public static AssetBuildEditor Instance;
    private BuildConfig curBuildConfig = new BuildConfig();

    private Vector2 confScrollPos;

    private static string[] Strategys = new []
    {
        "Scene" , "Pre File"  , "Pre Direction" , "AllInOne"
    };

    
    /// <summary>
    /// 资源类型
    /// </summary>
    private static string[] AssetTypes = {"Scene" , "Model" , "Effect" , "UI" , "GameObject"};



    private Dictionary<string , IStrategy> strategyDic = new Dictionary<string, IStrategy>();
    private List<string> buildConfigs = new List<string>(new []{"None" }); 
    
    private bool isAddNewConfig;

    /// <summary>
    /// Bundle引用的公共资源
    /// </summary>
    private Dictionary<string, List<string>> buildRefDic = new Dictionary<string, List<string>>();
    
    public void Initialize()
    {

        Instance = this;
        strategyDic[Strategys[0]] = new SceneStrategy();
        strategyDic[Strategys[1]] = new PreFileStrategy();
        strategyDic[Strategys[2]] = new PreDirectionStrategy();
        strategyDic[Strategys[3]] = new AllInOneStrategy();

        BuildConfigManager.Instance.ReadConfig();

        BuildConfig[] configArr = BuildConfigManager.Instance.BuildConfigs;
        if (configArr.Length > 0)
        {
            curBuildConfig = configArr[0];
            foreach (BuildConfig bc in configArr)
                buildConfigs.Add(bc.BundleName);
        }
    }


    public static IStrategy GetStrategy(string strategy)
    {
        if(strategy == Strategys[0])
          return new SceneStrategy();
        if(strategy == Strategys[1])
            return new PreFileStrategy();
        if(strategy == Strategys[2])
            return new PreDirectionStrategy();
        if(strategy == Strategys[3])
            return new AllInOneStrategy();
        return null;
    }
    /// <summary>
    /// 添加引用记录
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="depAssetPath"></param>
    public void AddDependencie(string bundleName, string depAssetPath)
    {
        if(!buildRefDic.ContainsKey(bundleName))
            buildRefDic[bundleName] = new List<string>();

        if (buildRefDic[bundleName].Contains(depAssetPath)) return;

        buildRefDic[bundleName].Add(depAssetPath);
    }

    /// <summary>
    /// 分配Bundle
    /// </summary>
    /// <param name="bundleName">Bundle名称</param>
    /// <param name="assetPath">资源的绝对路径</param>
    /// <param name="buildConfigName">配置的名称</param>
    public List<string> AssignBundle(string bundleName, string assetPath , string buildConfigName)
    {
        string relativeScenePath = assetPath.Replace(Application.dataPath, "Assets");
        string[] depAssetArr = AssetDatabase.GetDependencies(relativeScenePath);

        string rootPath = Path.GetDirectoryName(assetPath).Replace("\\", "/");
        rootPath = rootPath.Replace(Application.dataPath, "Assets");

        List<string> allBundleFiles = new List<string>();
        foreach (string defAsset in depAssetArr)
        {
            string suffix = Path.GetExtension(defAsset);
            if (BuildGlobal.FilterAsset.Contains(suffix)) continue;
            
            if (defAsset.StartsWith(rootPath))
            {
                AssetBundleUtil.AssignBundle(defAsset, bundleName);
                allBundleFiles.Add(defAsset);
            }
            else
            {
                //记录引用，避免公共集合打包时，把不必要的资源也打要进来
                AddDependencie(buildConfigName, defAsset);
            }
        }

        //分配自身的Bundle
        AssetBundleUtil.AssignBundle(assetPath, bundleName);
        allBundleFiles.Add(assetPath);
        return allBundleFiles;
    }
    /// <summary>
    /// 获得Bundle资源引用列表信息
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public List<string> GetBundleRefList(string bundleName)
    {
        if (!buildRefDic.ContainsKey(bundleName)) return null;
        return buildRefDic[bundleName];
    } 

    void OnGUI()
    {
        //平台
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Build Target:", EditorStyles.boldLabel , GUILayout.MaxWidth(100));
        EditorGUILayout.EnumPopup(EditorUserBuildSettings.activeBuildTarget);
        GUILayout.EndHorizontal();
        
        this.onSettingGUI();
        
        this.onConfigListGUI();

        if (GUILayout.Button("Build" , GUILayout.Height(30)))
        {
            BuildConfig[] configArr = BuildConfigManager.Instance.BuildConfigs;

            IStrategy bcStrategy = null;
            BuildConfig lastBuildConfig = null;
            HashSet<string> bcLog = new HashSet<string>();
            // 分配AssetBundle关联
            foreach (BuildConfig buildConfig in configArr)
            {
                if (bcLog.Contains(buildConfig.BundleName)) continue;

                lastBuildConfig = onPrebuild(buildConfig);

                bcStrategy = GetStrategy(lastBuildConfig.BuildStrategy);
                bcStrategy.BeginProcess(lastBuildConfig);
                bcLog.Add(lastBuildConfig.BundleName);
            }

            this.buildAllBundle();

            //打包完成后的处理，比如文件移动等
            foreach (BuildConfig buildConfig in configArr)
            {
                bcStrategy = GetStrategy(buildConfig.BuildStrategy);
                bcStrategy.EndProcess(buildConfig);
            }

            //保存
            BuildConfigManager.Instance.SaveConfig();

            //提示
            EditorUtility.DisplayDialog("提示", "已打包完成！", "OK");
        }
        GUILayout.Space(5);
    }


    /// <summary>
    /// 前置处理
    /// </summary>
    /// <param name="buildConfig"></param>
    /// <returns></returns>
    private BuildConfig onPrebuild(BuildConfig buildConfig)
    {
        if (buildConfig.PreBuild == "None") return buildConfig;
        
        BuildConfig preBuild = BuildConfigManager.Instance.GetBuildConfig(buildConfig.PreBuild);

        return onPrebuild(preBuild);
    }


    /// <summary>
    /// 设置信息
    /// </summary>
    private void onSettingGUI()
    {
        EditorGUILayout.LabelField("Setting", EditorStyles.boldLabel);
        if (curBuildConfig == null) return;
        GUILayout.BeginHorizontal();
        GUILayout.Label("AssetBundle Name" , GUILayout.Width(120));
        curBuildConfig.BundleName = GUILayout.TextField(curBuildConfig.BundleName);
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Input Path" , EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        curBuildConfig.InputDir = GUILayout.TextField(curBuildConfig.InputDir);

        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string selectDir = EditorUtility.OpenFolderPanel("Input Directory", Application.dataPath, "input");
            curBuildConfig.InputDir = selectDir.Replace(Application.dataPath, "").Substring(1);
        }
        GUILayout.EndHorizontal();

        //search file suffix
        GUILayout.BeginHorizontal();
        GUILayout.Label("Include file" , GUILayout.MaxWidth(120));
        curBuildConfig.FileSuffixs = GUILayout.TextField(curBuildConfig.FileSuffixs);
        GUILayout.EndHorizontal();

        //Output Path
        EditorGUILayout.LabelField("Output Path" , EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        curBuildConfig.OutputDir = GUILayout.TextField(curBuildConfig.OutputDir);

        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string selectDir = EditorUtility.OpenFolderPanel("Output Directory", BuildGlobal.EXPORT_BUNDLE_ROOT , "output");
            curBuildConfig.OutputDir = selectDir.Replace(BuildGlobal.EXPORT_BUNDLE_ROOT.Replace("\\" , "/"), "");
        }
        GUILayout.EndHorizontal();
        
        // Asset Type
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset Type", GUILayout.MaxWidth(100));

        int selectAssetTyp = 0;
        if (!string.IsNullOrEmpty(curBuildConfig.AssetType))
        {
            for (int i = 0; i < AssetTypes.Length; i++)
            {
                if (curBuildConfig.AssetType == AssetTypes[i])
                {
                    selectAssetTyp = i;
                    break;
                }
            }            
        }
        selectAssetTyp = EditorGUILayout.Popup(selectAssetTyp, AssetTypes);
        curBuildConfig.AssetType = AssetTypes[selectAssetTyp];
        GUILayout.EndHorizontal();

        //前置条件
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pre Build", GUILayout.MaxWidth(100));

        int selectPrebuildIndex = buildConfigs.FindIndex((s) =>
        {
            return s == curBuildConfig.PreBuild;
        });

        selectPrebuildIndex = EditorGUILayout.Popup(selectPrebuildIndex < 0 ? 0 : selectPrebuildIndex, buildConfigs.ToArray());
        curBuildConfig.PreBuild = buildConfigs[selectPrebuildIndex];
        GUILayout.EndHorizontal();

        //build strategy
        EditorGUILayout.LabelField("Build Strategy" , EditorStyles.boldLabel);
        int selectStratgyIndex = 0;
        for (int i = 0; i < Strategys.Length; i++)
        {
            if (Strategys[i] == curBuildConfig.BuildStrategy)
            {
                selectStratgyIndex = i;
                break;
            }
        }
        GUILayout.BeginHorizontal();
        selectStratgyIndex = EditorGUILayout.Popup(selectStratgyIndex, Strategys);
        curBuildConfig.BuildStrategy = Strategys[selectStratgyIndex];

        curBuildConfig.OptionSerach = (SearchOption)EditorGUILayout.EnumPopup(curBuildConfig.OptionSerach , GUILayout.MaxWidth(130));
        GUILayout.EndHorizontal();


        //save change
        if (isAddNewConfig)
        {
             using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Save Configs"))
                {
                    BuildConfigManager.Instance.SaveConfig();
                    isAddNewConfig = false;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.FlexibleSpace();
            }           
        }

    }

    /// <summary>
    /// 配置列表
    /// </summary>
    private void onConfigListGUI()
    {
        GUILayout.Space(5);

        BuildConfig[] configArr = BuildConfigManager.Instance.BuildConfigs;

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Configs", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add"))
            {
                BuildConfigManager.Instance.AddBuildConfig("default" + configArr.Length);
                isAddNewConfig = true;
            }
        }

        GUILayout.Space(5);
        confScrollPos = GUILayout.BeginScrollView(confScrollPos);
        foreach (BuildConfig buildConfig in configArr)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width * 0.1f);
            if (curBuildConfig != null && curBuildConfig.BundleName == buildConfig.BundleName)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button(buildConfig.BundleName))
            {
                curBuildConfig = buildConfig;
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(Screen.width * 0.1f);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.Space(5);
    }


    /// <summary>
    /// 打包所有的Bundle文件
    /// </summary>
    private void buildAllBundle()
    {
        if (AppConst.LuaBundleMode)
            buildAllBundle(BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        else
        {
            //非Bundle模式，生成文件名同路径的映射
            StringBuilder buf = new StringBuilder();

            buf.AppendLine(genDevAssetForMap());
            buf.AppendLine(AppPathUtils.AssetForMap);
            buf.AppendLine(genAssetForBundleMap());
            //写入映射关联文件
            string mapPath = Path.Combine(AppPathUtils.PersistentDataRootPath, "dev" + AppPathUtils.AssetBundleMap);
            File.WriteAllText(mapPath, buf.ToString());

            AssetDatabase.Refresh();
        }
    }


    private void buildAllBundle(BuildAssetBundleOptions options , BuildTarget target)
    {
        string path = BuildGlobal.EXPORT_BUNDLE_ROOT;
        BuildPipeline.BuildAssetBundles(path, options, target);

        //拷贝文件
        CopyFileTo(AppPathUtils.PersistentDataRootPath);

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 拷贝文件到运行环境目录
    /// </summary>
    public static void CopyFileTo(string targetDir)
    {
        //对AssetBundleManifest文件改名，默认没有后缀
        string manifestFile = BuildGlobal.EXPORT_BUNDLE_ROOT.Replace("\\" , "/");
        manifestFile = manifestFile.Substring(0, manifestFile.Length - 1);
        manifestFile = manifestFile.Substring(manifestFile.LastIndexOf("/") + 1);
        manifestFile = BuildGlobal.EXPORT_BUNDLE_ROOT + manifestFile;

        File.Copy(manifestFile, BuildGlobal.EXPORT_BUNDLE_ROOT + "bin_none" + AppPathUtils.BundleSuffix, true);

        //再次移动所有的AssetBundle文件到目标目录
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

        string[] allFiles = Directory.GetFiles(BuildGlobal.EXPORT_BUNDLE_ROOT, "*"+AppPathUtils.BundleSuffix,
            SearchOption.AllDirectories);

        string mapPath = Path.Combine(targetDir , AppPathUtils.AssetBundleMap);
        //读取原映射文件
        Dictionary<string, string> mapDic = new Dictionary<string, string>();
        if (File.Exists(mapPath))
        {
            string[] mapInfos = File.ReadAllText(mapPath).Trim().Split('\n');
            foreach (string mapInfo in mapInfos)
            {
                if (mapInfo.IndexOf(':') >= 0) break;
                string[] maps = mapInfo.Split(';');
                mapDic[maps[0]] = maps[1];
            }            
        }

        StringBuilder fileMap = new StringBuilder();
        foreach (string filePath in allFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            string[] fileAndType = fileName.Split('_');
            string fileType = "none";
            if (fileAndType.Length > 1) fileType = fileAndType[fileAndType.Length - 1];

            string relativePath = filePath.Replace(BuildGlobal.EXPORT_BUNDLE_ROOT, "");
            relativePath = relativePath.Replace("_" + fileType, "");
            string md5 = Util.md5file(filePath);

            //删除旧的文件
            bool isCopy = false;
            if (mapDic.ContainsKey(filePath) && mapDic[filePath] != md5)
            {
                if(File.Exists(mapDic[filePath]))
                    File.Delete(mapDic[filePath]);
                isCopy = true;
            }

            string targetPath = Path.Combine(targetDir, md5);
            if(isCopy || !File.Exists(targetPath))
                File.Copy(filePath ,  targetPath, true);

            fileMap.AppendFormat("{0};{1};{2}\n" , fileType.ToLower(), relativePath, md5);
        }

        //添加资源映射
        fileMap.AppendLine(AppPathUtils.AssetForMap);
        fileMap.Append(genAssetForBundleMap());

        //写入映射关联文件
        File.WriteAllText(mapPath, fileMap.ToString());
    }

    /// <summary>
    /// 生成资源同Bundle文件的映射，进行在加载资源时，识别包含此文件的Bundle名称
    /// </summary>
    private static string genAssetForBundleMap()
    {
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        StringBuilder buf = new StringBuilder();
        
        foreach (string bundleName in bundleNames)
        {
            string[] assetPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            string fileType = "none";
            string[] fileAndType = bundleName.Replace(AppPathUtils.BundleSuffix, "").Split('_');
            if (fileAndType.Length > 1) fileType = fileAndType[fileAndType.Length - 1];

            foreach (string assetPath in assetPathArr)
            {
                string suffix = Path.GetExtension(assetPath);
                if(!BuildGlobal.MapIndexFilter.Contains(suffix))    continue;

                string fileName = Path.GetFileName(assetPath);
                buf.AppendFormat("{0};{1};{2}\n", fileType.ToLower(), fileName, bundleName );
            }
        }

        return buf.ToString();
    }

    /// <summary>
    /// 生成开发期的映射数据，文件类型|AssetBundleName|Resource相对工程路径
    /// </summary>
    /// <returns></returns>
    private static string genDevAssetForMap()
    {
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        StringBuilder buf = new StringBuilder();

        foreach (string bundleName in bundleNames)
        {
            string[] assetPathArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            string fileType = "none";
            string[] fileAndType = bundleName.Replace(AppPathUtils.BundleSuffix, "").Split('_');
            if (fileAndType.Length > 1) fileType = fileAndType[fileAndType.Length - 1];

            foreach (string assetPath in assetPathArr)
            {
                string suffix = Path.GetExtension(assetPath);
                if (!BuildGlobal.MapIndexFilter.Contains(suffix)) continue;

                string bundle = bundleName.Replace("_" + fileType.ToLower(), "");
                buf.AppendFormat("{0};{1};{2}\n", fileType.ToLower(), bundle, assetPath);
            }
        }

        return buf.ToString();
    }
}
