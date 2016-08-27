/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 描述：逐目录策略
/// <para>创建时间：2016-06-24</para>
/// </summary>
public class PreDirectionStrategy : IStrategy {

    public void BeginProcess(BuildConfig buildConfig)
    {
        string absolutionPath = Path.Combine(Application.dataPath, buildConfig.InputDir);
        string[] findFile = buildConfig.FileSuffixs.Split('|');
        
        //查找打包输入路径下的根目录列表
        string[] dirArr = Directory.GetDirectories(absolutionPath, "*", SearchOption.TopDirectoryOnly);

        foreach (string subDir in dirArr)
        {
            DirectoryInfo di = new DirectoryInfo(subDir);
            //将文件夹的名字做为Bundle名称
            AssetBundleUtil.MulInOneBundle(di.Name , buildConfig.AssetType , subDir , findFile , buildConfig.OptionSerach);
        }
    }

    public void EndProcess(BuildConfig buildConfig)
    {
        
        Debugger.Log("<<PreDirection>> End ! build config name is " + buildConfig.BundleName);
    }
}
