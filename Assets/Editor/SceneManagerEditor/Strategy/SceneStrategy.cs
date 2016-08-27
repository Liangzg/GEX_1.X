/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

/// <summary>
/// 描述：场景打包策略
/// <para>创建时间：2016-06-27</para>
/// </summary>
public class SceneStrategy : IStrategy
{

    private BuildConfig curBuildConfig;
    public void BeginProcess(BuildConfig buildConfig)
    {
        curBuildConfig = buildConfig;

        string absolutionInputPath = Path.Combine(Application.dataPath, buildConfig.InputDir);
        string[] sceneFilePathArr = System.IO.Directory.GetFiles(absolutionInputPath, "*.unity",SearchOption.AllDirectories);

        foreach (string scenePath in sceneFilePathArr)
        {
            string scenePrefab = scenePath.Replace(".unity", ".prefab");
            scenePrefab = scenePrefab.Replace("\\" , "/");

            if(!File.Exists(scenePrefab))   continue;

            assignBundle(AssetBundleUtil.FormatOutputPath(buildConfig) , scenePrefab);
        }
    }

    /// <summary>
    /// 分配Bundle
    /// </summary>
    /// <param name="outputPath">输出路径</param>
    /// <param name="scenePrefabPath">资源的绝对路径</param>
    private void assignBundle(string outputPath , string scenePrefabPath)
    {
        string sceneName = Path.GetFileNameWithoutExtension(scenePrefabPath);

        string relativeScenePath = scenePrefabPath.Replace(Application.dataPath, "Assets");
        string[] depAssetArr = AssetDatabase.GetDependencies(relativeScenePath);

        string rootPath = Path.GetDirectoryName(scenePrefabPath).Replace("\\" , "/");
        rootPath = rootPath.Replace(Application.dataPath, "Assets");

        string relativePath = outputPath.Replace(BuildGlobal.EXPORT_BUNDLE_ROOT, "");
        string bundleName = relativePath + "/"  + sceneName + "_" + curBuildConfig.AssetType;
        foreach (string defAsset in depAssetArr)
        {
            string suffix = Path.GetExtension(defAsset);
            if(BuildGlobal.FilterAsset.Contains(suffix))    continue;
            
            
            if (defAsset.StartsWith(rootPath))
            {
                AssetBundleUtil.AssignBundle(defAsset , bundleName);
            }
            else
            {
                //记录引用，避免公共集合打包时，把不必要的资源也打要进来
                AssetBuildEditor.Instance.AddDependencie(curBuildConfig.BundleName, defAsset);

//                //如果依赖是资源处于公共资源目录
//                //检查资源是否被分配，如果没有则依据目录进行分配
//                BuildConfig buildConf = BuildConfigManager.Instance.FindAssetBuildConfig(defAsset);
//                if (buildConf == null)
//                {
//                    Debug.LogWarning("<<SceneStrategy , assignBundle>> Cant find build config ! asset path is " + defAsset);
//                    continue;
//                }
//
//                string dirName = buildConf.InputDir.Substring(buildConf.InputDir.LastIndexOf("/") + 1);
//                if (string.IsNullOrEmpty(dirName))
//                {
//                    if(buildConf.InputDir.IndexOf("/") < 0)
//                        dirName = buildConf.InputDir;
//                }
//                AssetBundleUtil.AssignBundle(defAsset , dirName);
            }
        }

        //分配自身的Bundle
        AssetBundleUtil.AssignBundle(scenePrefabPath , bundleName);
    }

    public void EndProcess(BuildConfig buildConfig)
    {

    }
}
