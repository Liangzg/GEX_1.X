/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using GOE.Scene;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

/// <summary>
/// 描述：动态烘焙场景光照模型，并产生对应的Prefab文件
/// <para>创建时间：2016-06-15</para>
/// </summary>
public sealed class LightMapEditor
{

    private const string LightMapsDir = "Resources/Lightmaps/";

    private static List<RemapTexture2D> sceneLightmaps = new List<RemapTexture2D>();

    public static void UpdateLightmaps()
    {
        PrefabLightmapData pld = GameObject.FindObjectOfType<PrefabLightmapData>();
        if (pld == null) return;

        LightmapSettings.lightmaps = null;
        PrefabLightmapData.ApplyLightmaps(pld.mRendererInfos , pld.mLightmapFars , pld.mLightmapNears);

        Debug.Log("Prefab Lightmap updated");
    }

    public static void GenLightmap()
    {
        genBakeLightmapAndPrefab(false);

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("----------------Update to Prefab Lightmap Finish -------------------------");
    }

    /// <summary>
    /// 生成lightmap和prefab资源
    /// </summary>
    /// <param name="isQuiet">是否静默执行，true不会出现异常提示</param>
    private static void genBakeLightmapAndPrefab(bool isQuiet)
    {
        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }
        Debug.ClearDeveloperConsole();

        PrefabLightmapData[] pldArr = GameObject.FindObjectsOfType<PrefabLightmapData>();
        if (pldArr == null || pldArr.Length <= 0)
        {
            if(!isQuiet)
                EditorUtility.DisplayDialog("提示", "没有找到必要的脚本PrefabLightmapData，请检查场景", "OK");
            return;
        }

        Lightmapping.Bake();
        sceneLightmaps.Clear();

        string path = Path.Combine(Application.dataPath, LightMapsDir);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        Scene curScene = EditorSceneManager.GetActiveScene();
        string sceneName = Path.GetFileNameWithoutExtension(curScene.name);
        string scenePath = Path.GetDirectoryName(curScene.path) + "/" + sceneName + "/";
        string resourcesPath = Path.GetDirectoryName(curScene.path) + "/" + sceneName + "_lightmap/" + sceneName;

        foreach (PrefabLightmapData pld in pldArr)
        {
            GameObject gObj = pld.gameObject;
            List<RendererInfo> renderers = new List<RendererInfo>();
            List<Texture2D> lightmapFars = new List<Texture2D>();
            List<Texture2D> lightmapNears = new List<Texture2D>();

            genLightmapInfo(scenePath, resourcesPath, gObj, renderers, lightmapFars, lightmapNears);

            pld.mRendererInfos = renderers.ToArray();
            pld.mLightmapFars = lightmapFars.ToArray();
            pld.mLightmapNears = lightmapNears.ToArray();

            GameObject targetPrefab = PrefabUtility.GetPrefabParent(gObj) as GameObject;

            //生成拷贝副本
            GameObject cloneGObj = (GameObject)GameObject.Instantiate(gObj);
            //删除过滤结点
            FilterSceneFlag[] flags = cloneGObj.GetComponentsInChildren<FilterSceneFlag>();
            foreach (FilterSceneFlag flag in flags)
                flag.ClearSelf();

            if (targetPrefab != null)
            {
                //自定义存放的路径
                PrefabUtility.ReplacePrefab(cloneGObj, targetPrefab);
            }
            else
            {
                //默认路径
                string prefabPath = Path.GetDirectoryName(curScene.path) + "/" + sceneName + ".prefab";
                PrefabUtility.CreatePrefab(prefabPath, cloneGObj, ReplacePrefabOptions.ConnectToPrefab);
            }
            GameObject.DestroyImmediate(cloneGObj);

            //改变当前场景中的光照贴图信息
            PrefabLightmapData.ApplyLightmaps(pld.mRendererInfos, pld.mLightmapFars, pld.mLightmapNears);
        }
    }


    /// <summary>
    /// 生成所有场景的Prefab资源
    /// </summary>
    public static void GenLightmapPrefabAll()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();

        string[] sceneDirs = new[] {"Assets/AllResources/scene-s/"};
        string curScenePath = EditorSceneManager.GetActiveScene().path;
        foreach (string sceneDir in sceneDirs)
        {
            string[] sceneArr = Directory.GetFiles(sceneDir, "*.unity" , SearchOption.AllDirectories);
            if(sceneArr == null || sceneArr.Length <= 0)    continue;
            int index = 0;
            foreach (string scenePath in sceneArr)
            {
                EditorUtility.DisplayProgressBar("生成Lightmap Prefab", "正在打包生成资源，请等待...", index / (float)sceneArr.Length);
                
                Scene curScene = EditorSceneManager.OpenScene(scenePath.Replace("\\", "/"));
                SceneManager.SetActiveScene(curScene);

                genBakeLightmapAndPrefab(true);
                
                index++;
            }
        }

        EditorUtility.ClearProgressBar();

        //还原打包前打开的场景 
        EditorSceneManager.OpenScene(curScenePath);

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("已完成打包" + System.DateTime.Now.ToString("HH:mm:ss"));
        EditorUtility.DisplayDialog("提示", "已完成批量打包!" + System.DateTime.Now.ToString("HH:mm:ss"), "OK");
    }


    private static void genLightmapInfo(string scenePath , string resourcePath , GameObject root, 
                                        List<RendererInfo> renderers, List<Texture2D> lightmapFars,
                                        List<Texture2D> lightmapNears)
    {
        MeshRenderer[] subRenderers = root.GetComponentsInChildren<MeshRenderer>();

        LightmapData[] srcLightData = LightmapSettings.lightmaps;

        foreach (MeshRenderer meshRenderer in subRenderers)
        {
            if(meshRenderer.lightmapIndex == -1)    continue;

            RendererInfo renderInfo = new RendererInfo();
            renderInfo.renderer = meshRenderer;
            renderInfo.LightmapIndex = meshRenderer.lightmapIndex;
            renderInfo.LightmapOffsetScale = meshRenderer.lightmapScaleOffset;

            Texture2D lightmapFar = srcLightData[meshRenderer.lightmapIndex].lightmapFar;
            Texture2D lightmapNear = srcLightData[meshRenderer.lightmapIndex].lightmapNear;

            int sceneCacheIndex = addLightmap(scenePath, resourcePath, renderInfo.LightmapIndex, lightmapFar,
                lightmapNear);

            renderInfo.LightmapIndex = lightmapFars.IndexOf(sceneLightmaps[sceneCacheIndex].LightmapFar);
            if (renderInfo.LightmapIndex == -1)
            {
                renderInfo.LightmapIndex = lightmapFars.Count;
                lightmapFars.Add(sceneLightmaps[sceneCacheIndex].LightmapFar);
                lightmapNears.Add(sceneLightmaps[sceneCacheIndex].LightmapNear);
            }

            renderers.Add(renderInfo);
        }
    }


    private static int addLightmap(string scenePath, string resourcePath, int originalLightmapIndex,
        Texture2D lightmapFar,
        Texture2D lightmapNear)
    {

        for (int i = 0; i < sceneLightmaps.Count; i++)
        {
            if (sceneLightmaps[i].OriginalLightmapIndex == originalLightmapIndex)
            {
                return i;
            }
        }


        RemapTexture2D remapTex = new RemapTexture2D();
        remapTex.OriginalLightmapIndex = originalLightmapIndex;
        remapTex.OrginalLightmap = lightmapFar;

        string fileName = scenePath + "Lightmap-" + originalLightmapIndex;
        remapTex.LightmapFar = getLightmapAsset(fileName + "_comp_light.exr", resourcePath + "_light",
            originalLightmapIndex);

        if(lightmapNear != null)
            remapTex.LightmapNear = getLightmapAsset(fileName + "_comp_dir.exr", resourcePath + "_dir",
                originalLightmapIndex);

        sceneLightmaps.Add(remapTex);

        return sceneLightmaps.Count - 1;
    }


    private static Texture2D getLightmapAsset(string fileName, string resourecPath, int originalLightmapIndex)
    {
        TextureImporter importer = AssetImporter.GetAtPath(fileName) as TextureImporter;
        if (importer == null)   return null;
        importer.isReadable = true;
        AssetDatabase.ImportAsset(fileName , ImportAssetOptions.ForceUpdate);

        Texture2D assetLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(fileName);
        string assetPath = resourecPath + "_" + originalLightmapIndex + ".asset";
        Texture2D newLightmap = GameObject.Instantiate<Texture2D>(assetLightmap);

        string dir = Path.GetDirectoryName(assetPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

//        byte[] bytes = newLightmap.EncodeToPNG();
//        File.WriteAllBytes(assetPath, bytes);
        AssetDatabase.CreateAsset(newLightmap , assetPath);


        newLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        importer.isReadable = false;
        AssetDatabase.ImportAsset(assetPath , ImportAssetOptions.ForceUpdate);
        return newLightmap;
    }




}
