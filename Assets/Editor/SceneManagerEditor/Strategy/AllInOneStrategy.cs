/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 描述：多个文件打包成一个集合
/// <para>创建时间：2016-06-28</para>
/// </summary>
public class AllInOneStrategy : IStrategy {

    public void BeginProcess(BuildConfig buildConfig)
    {
        string absolutionPath = Path.Combine(Application.dataPath, buildConfig.InputDir);
        string[] findFile = buildConfig.FileSuffixs.Split('|');
        List<string> files = new List<string>();

        foreach (string fileSuffix in findFile)
        {
            string[] fileArr = Directory.GetFiles(absolutionPath, fileSuffix, buildConfig.OptionSerach);
            files.AddRange(fileArr);
        }
        
        //为文件分配到指定Bundle
        List<string> allBundleFile = new List<string>();
        string bundleName = buildConfig.BundleName + "_" + buildConfig.AssetType;
        foreach (string filePath in files)
        {
            string relativePath = filePath.Replace("\\", "/");
            relativePath = relativePath.Replace(Application.dataPath, "Assets");

            List<string> bundleFiles = AssetBuildEditor.Instance.AssignBundle(bundleName, relativePath , buildConfig.BundleName);
            allBundleFile.AddRange(bundleFiles);
        }

        //如果存在PreBuild , 则有可能是公共目录资源，同时检查资源冗余
        if (string.IsNullOrEmpty(buildConfig.PreBuild) || buildConfig.PreBuild == "None") return;

        List<string> refAssets = AssetBuildEditor.Instance.GetBundleRefList(buildConfig.BundleName);
        foreach (string asset in refAssets)
            allBundleFile.Remove(asset);

        //清理多余的资源
        foreach (string asset in allBundleFile)
        {
            AssetBundleUtil.ClearBundle(asset);
        }
    }

    public void EndProcess(BuildConfig buildConfig)
    {
        
    }
}
