/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 描述：逐文件策略
/// <para>创建时间：2016-06-24</para>
/// </summary>
public class PreFileStrategy : IStrategy {

    public void BeginProcess(BuildConfig buildConfig)
    {
        string absolutionPath = Path.Combine(Application.dataPath, buildConfig.InputDir);
        string[] findFile = buildConfig.FileSuffixs.Split('|');

        //查找目录文件
        List<string> files = new List<string>();
        foreach (string fileSuffix in findFile)
        {
            string[] fileArr = Directory.GetFiles(absolutionPath, fileSuffix, buildConfig.OptionSerach);
            files.AddRange(fileArr);
        }

        //分配文件Bundle
        foreach (string filePath in files)
        {
            if(filePath.EndsWith(BuildGlobal.Meta))    continue;

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            string relativePath = filePath.Replace("\\", "/");
            relativePath = relativePath.Replace(Application.dataPath, "Assets");

            AssetBuildEditor.Instance.AssignBundle(fileName, relativePath, fileName);
        }
    }

    public void EndProcess(BuildConfig buildConfig)
    {
        Debugger.Log("<<PreFileStrategy>> End ! build config name is " + buildConfig.BundleName);
    }
}
