/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 描述：资源工具集的菜单目录
/// <para>创建时间：2016-06-30</para>
/// </summary>
public class AssetToolsMenu  {

    [MenuItem("AssetTools/Asset Build Editor")]
    public static void ShowEditor()
    {
        AssetBuildEditor abEditor = EditorWindow.GetWindow<AssetBuildEditor>();
        abEditor.Initialize();
        abEditor.Show();
    }

    [MenuItem("AssetTools/Copy Assets To Install")]
    public static void CopyInstallAsset()
    {
        AssetBuildEditor.CopyFileTo(Application.streamingAssetsPath);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "文件已拷贝完成", "确定");
    }

    [MenuItem("AssetTools/Copy Assets To RunBin")]
    public static void CopyRunBinAsset()
    {
        AssetBuildEditor.CopyFileTo(AppPathUtils.PersistentDataRootPath);
        EditorUtility.DisplayDialog("提示", "文件已拷贝完成", "确定");
    }

    [MenuItem("AssetTools/Lightmap/Gen Lightmap Prefab")]
    public static void GenLightmap()
    {
        LightMapEditor.GenLightmap();
    }

    [MenuItem("AssetTools/Lightmap/Update Scene Prefab Lightmaps")]
    public static void UpdateLightmaps()
    {
        LightMapEditor.UpdateLightmaps();
    }

    [MenuItem("AssetTools/Lightmap/Gen Lightmap Prefab All")]
    public static void GenLightmapPrefabAll()
    {
        LightMapEditor.GenLightmapPrefabAll();
    }

}
