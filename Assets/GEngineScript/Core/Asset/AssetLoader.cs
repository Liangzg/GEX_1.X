/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GOE.Scene;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 描述：资源加载器，主要用于加载除场景资源外的其它资源
/// <para>创建时间：2016-06-20</para>
/// </summary>
public class AssetLoader : ASignalEntry<AssetLoader>{
    public enum EAssetType
    {
        ALL,
        Scene, //场景 
        Model , //模型 
        Effect , //特效
        UI, //UI界面
        GameObject, //普通GameObject
    }


    public enum EAssetBaseType
    {
        Text , ByteArray , Texture2D , AssetBundle
    }

    private Dictionary<string , ARCCache<string , Asset>>  bundlePoolCache = new Dictionary<string, ARCCache<string, Asset>>(); 
    /// <summary>
    /// key:Name Value:Path 
    /// </summary>
    private Dictionary<string , string> nameToPathMap = new Dictionary<string, string>();
    private Dictionary<string , string> pathToBundleMap = new Dictionary<string, string>(); 
    
    private AssetBundleManifest manifest;
    /// <summary>
    /// 是否是加载Bundle资源
    /// </summary>
    public static bool isBundle = true;

    private AssetLoader()
    {
        swapnCachePool(EAssetType.ALL.ToString(), 100);
    }

    #region --------------Lua层的静态调用方法-----------------------
    /// <summary>
    /// 通过指定资源名加载GameObject资源
    /// </summary>
    /// <param name="resName">资源名称</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadGameObjectByName(string resName, EAssetType assetType , Action<GameObject> callback)
    {
        AppInter.StartCoroutine(Instance.SwapAssetByName<UnityEngine.Object>(EAssetType.ALL.ToString() , resName , assetType, (obj) =>
        {
            if (obj == null)
            {
                callback.Invoke(null);
                return;
            }
            //实例
            GameObject gObj = GameObject.Instantiate(obj) as GameObject;
            callback.Invoke(gObj);
        }));
    }



    /// <summary>
    /// 通过指定的UI资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">UI资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadUI(string resName, Action<GameObject> callback)
    {
        LoadGameObjectByName(resName , EAssetType.UI, callback);
    }
    /// <summary>
    /// 通过指定的特效资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">特效资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadEffect(string resName, Action<GameObject> callback)
    {
        LoadGameObjectByName(resName, EAssetType.Effect, callback);
    }
    /// <summary>
    /// 通过指定的模型资源名加载实例GameObject对象
    /// </summary>
    /// <param name="resName">模型资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadModel(string resName, Action<GameObject> callback)
    {
        LoadGameObjectByName(resName, EAssetType.Model, callback);
    }

    /// <summary>
    /// 加载一个普通的GameObject对象
    /// </summary>
    /// <param name="resName">模型资源名</param>
    /// <param name="callback">加载实例完成后的回调</param>
    public static void LoadGameObject(string resName, Action<GameObject> callback)
    {
        LoadGameObjectByName(resName, EAssetType.GameObject, callback);
    }
    /// <summary>
    /// 通过指定路径加载AssetBundle中的Text资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    //    public static void LoadTextAtPath(string path, Action<string> callback)
    //    {
    //        AssetLoader loader = AssetLoader.Instance;
    //        loader.swapnCachePool(EAssetType.ALL.ToString(), 100);
    //        loader.SwapAssetAtPath(EAssetType.ALL.ToString(), path, (obj) =>
    //        {
    //            if (obj == null)
    //            {
    //                callback(null);
    //                return;
    //            }
    //            TextAsset textAss = obj as TextAsset;
    //            callback(textAss.text);
    //        });
    //    }

    /// <summary>
    /// 通过指定资源名称加载AssetBundle中的Text资源
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    //    public static void LoadTextByName(string resName, Action<string> callback)
    //    {
    //        AssetLoader loader = AssetLoader.Instance;
    //        string path = loader.GetAssetPath(resName);
    ////        LoadTextAtPath(path, callback);
    //    }

    /// <summary>
    /// 通过指定路径加载AssetBundle中的Text资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public static void LoadByteArrayAtPath(string path, Action<byte[]> callback)
    {
//       Instance.SwapAssetAtPath(EAssetType.ALL.ToString(), path, (obj) =>
//        {
//            if (obj == null)
//            {
//                callback(null);
//                return;
//            }
//            //todo
////            TextAsset textAss = obj as TextAsset;
////            callback(textAss.text);
//        });
    }

//    /// <summary>
//    /// 通过指定资源名称加载AssetBundle中的Text资源
//    /// </summary>
//    /// <param name="resName"></param>
//    /// <param name="callback"></param>
//    public static void LoadByteArrayByName(string resName, Action<byte[]> callback)
//    {
//        AssetLoader loader = AssetLoader.Instance;
//        string path = loader.GetAssetPath(resName);
//        //todo
////        LoadTextAtPath(path, callback);
//    }

    /// <summary>
    /// 加载场景资源
    /// </summary>
    /// <param name="sceneName">场景的名称</param>
    /// <param name="callback">场景实例完成的回调</param>
    public static IEnumerator LoadScene(string sceneName, Action<GameObject> callback , Action<float, float> progress)
    {
        if (!sceneName.EndsWith(".prefab")) sceneName += ".prefab";

        return Instance.SwapAssetByName<UnityEngine.Object>(EAssetType.ALL.ToString(), sceneName, EAssetType.Scene, (obj) =>
        {
            if (obj == null)
            {
                if(callback != null)    callback.Invoke(null);
                return;
            }

            //查找当前场景的场景结点，进行删除
            PrefabLightmapData curScene = GameObject.FindObjectOfType<PrefabLightmapData>();
            if (curScene != null)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(curScene.gameObject);
#else
                GameObject.Destroy(curScene.gameObject);
#endif
            }

            //实例新场景
            GameObject gObj = GameObject.Instantiate(obj) as GameObject;
            if (callback != null) callback.Invoke(gObj);
        } , progress);
    }

#endregion


#region ----------私有方法---------------
    /// <summary>
    /// 获得CachePool池
    /// </summary>
    /// <param name="poolName">池的名称</param>
    /// <returns>如果不存在指定池，则返回null</returns>
    private ARCCache<string, Asset> getCachePool(string poolName)
    {
        if (!bundlePoolCache.ContainsKey(poolName)) return null;

        return bundlePoolCache[poolName];
    }

    /// <summary>
    /// 从缓存链中获取一个指定名称的缓存池
    /// </summary>
    /// <param name="poolName">池的名称</param>
    /// <param name="cacheCount">缓存池的大小</param>
    /// <returns>如果缓存链中不存在，则默认会创建</returns>
    private ARCCache<string, Asset> swapnCachePool(string poolName , int cacheCount = 10)
    {
        if (bundlePoolCache.ContainsKey(poolName)) return bundlePoolCache[poolName];

        ARCCache<string , Asset> cache = new ARCCache<string, Asset>(cacheCount);
        //释放清理
        cache.DestroyCallback = bundle =>
        {
            bundle.Value.Unload(false);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        };
        bundlePoolCache[poolName] = cache;
        return cache;
    }

    /// <summary>
    /// 加载AssetBundle及其依赖的资源
    /// </summary>
    /// <param name="cachePool">缓存池</param>
    /// <param name="bundleName">目标AssetBundle资源</param>
    /// <param name="callback">最终的资源加载完成时的回调</param>
    /// <returns></returns>
    private IEnumerator loadBundleAndDependenices(ARCCache<string, Asset> cachePool , string bundleName , 
                                                  Action<AssetBundle> callback, Action<float, float> progress)
    {
        string[] depAssetArr = manifest.GetAllDependencies(bundleName);
        foreach (string depAsset in depAssetArr)
        {
            //重新获取Bundle名，因为可能依赖的名称内带有目录信息
            string depBundleName = Path.GetFileName(depAsset);
            yield return AppInter.StartCoroutine(AsyncLoadBundleAtPath(depBundleName, (obj) =>
            {
                if (obj != null)
                {
                    Asset asset = new Asset(depBundleName, obj);
                    cachePool.Put(depBundleName, asset);
                }
            }));
        }

        yield return AppInter.StartCoroutine(AsyncLoadBundleAtPath(bundleName, callback , progress));
    }

    private IEnumerator loadBundleAndDependenices(ARCCache<string, Asset> cachePool, string bundleName,
        Action<AssetBundle> callback)
    {
        return loadBundleAndDependenices(cachePool, bundleName, callback, null);
    }
#endregion


    #region -----------公共访问方法---------------------
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path">资源的相对路径,相对于Assets的根目录 eg: CustomRes/XXX.prefab</param>
    /// <param name="assetType">其它的基础数据类型</param>
    /// <param name="callback">加载完成时的回调</param>
    /// <returns></returns>
    public IEnumerator AsyncLoadAtPath(string path ,EAssetBaseType assetType , Action<object> callback)
    {
        string finalPath = AppPathUtils.IsSeachExist(path , true);
        WWW _www = new WWW(finalPath);
        yield return _www;

        if (_www.error != null)
        {
            Debugger.LogError(_www.error + " , path -->> " + finalPath);
            callback(null);
            yield break;
        }

        object obj = null;
        if (assetType == EAssetBaseType.Text)
        {
            obj = _www.text;
        }else if (assetType == EAssetBaseType.ByteArray)
        {
            obj = _www.bytes;
        }else if (assetType == EAssetBaseType.Texture2D)
            obj = _www.texture;
        if(callback != null)    callback(obj);
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="bundName">未加密的Bundle文件名</param>
    /// <param name="callback">加载完成时的回调</param>
    public IEnumerator AsyncLoadBundleAtPath(string bundName, Action<AssetBundle> callback, Action<float, float> progress)
    {
        string finalPath = AppPathUtils.IsSeachExist(GetBundleEncrypeName(bundName), true);
        WWW _www = new WWW(finalPath);

        if (progress == null)
            yield return _www;
        else
        {
            int totalProgress = 0, curTotalProgress;
            int offset = 0;
            while (!_www.isDone)
            {
                curTotalProgress = (int)(_www.progress * 100);
                offset = curTotalProgress - totalProgress;
                totalProgress = curTotalProgress;
                progress.Invoke(offset, totalProgress);
                yield return null;
            }
            
            curTotalProgress = totalProgress == 0 ? (int)(_www.progress * 100) : totalProgress;
            offset = 100 - curTotalProgress;
            totalProgress = 100;
            progress.Invoke(offset, totalProgress);
            while (curTotalProgress < totalProgress)
            {
                curTotalProgress++;
                yield return null;
            }
            
            yield return null;
        }
        if (_www.error != null)
        {
            Debugger.LogError(_www.error + " , path -->> " + finalPath);
            callback(null);
            yield break;
        }
        
        if (callback != null) callback(_www.assetBundle);
    }

    public IEnumerator AsyncLoadBundleAtPath(string bundName, Action<AssetBundle> callback)
    {
        return AsyncLoadBundleAtPath(bundName, callback, null);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="resName">资源的名称 eg:res01.prefab</param>
    public void AsyncLoadBundleByName(string resName, EAssetType assetType , Action<AssetBundle> callback)
    {
        string finalName = GetBundleName(resName , assetType);
        if (string.IsNullOrEmpty(finalName))
        {
            Debugger.LogWarning("<<AssetLoader , SwapAssetByName>> Cant find file ! name :--->> " + resName + " , bundle :" + finalName);
            if (callback != null)
                callback(null);
            return;
        }
        
        AppInter.StartCoroutine(AsyncLoadBundleAtPath(finalName,  callback , null));
    }


    #region -----------Swap----------------
    //    /// <summary>
    //    /// 从缓存中获取资源
    //    /// </summary>
    //    /// <param name="poolName">缓存池的名称</param>
    //    /// <param name="path">资源相对路径</param>
    //    /// <param name="callback">加载完成的回调</param>
    //    public void SwapAssetAtPath(string poolName , string path , Action<UnityEngine.Object> callback)
    //    {
    //        ARCCache<string, AssetBundle> cachePool = swapnCachePool(poolName);
    //        AssetBundle bundle = cachePool.Get(path);
    //        if (bundle != null)
    //        {
    //            if (callback != null)
    //                callback(bundle.mainAsset);
    //            return;
    //        }
    //
    //        //异步加载
    //        AppInter.StartCoroutine(AsyncLoadAtPath(path,EAssetBaseType.AssetBundle, (obj) =>
    //        {
    //            bundle = obj as AssetBundle;
    //            if (bundle == null)
    //            {
    //                Debugger.LogWarning("<<AssetLoader , SwapAsset>> load faile !  Path -->> " + path);
    //                return;
    //            }
    //            cachePool.Put(path , bundle);
    //
    //            if(callback != null)
    //                callback(bundle.mainAsset);
    //        }));
    //    }

    /// <summary>
    /// 从AssetBundle缓存中获取资源
    /// </summary>
    /// <param name="poolName">缓存池的名称</param>
    /// <param name="resName">资源名</param>
    /// <param name="callback">加载完成的回调</param>
    public IEnumerator SwapAssetByName<T>(string poolName, string resName, EAssetType assetType, 
        Action<UnityEngine.Object> callback , Action<float, float> progress)
        where T : UnityEngine.Object
    {
        ARCCache<string, Asset> cachePool = swapnCachePool(poolName);
        string bundleName = GetBundleName(resName, assetType);
        if (string.IsNullOrEmpty(bundleName))
        {
            callback.Invoke(null);
            Debugger.LogWarning("<<AssetLoader , SwapAssetByName>> load faile !  Res -->> " + resName);
            yield break;
        }

        Asset bundle = cachePool.Get(bundleName);
#if UNITY_EDITOR
        if (!isBundle)
        {
            if (bundle == null)
            {
                string path = GetBundleEncrypeName(resName + assetType.ToString().ToLower());
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<T>(path);
                if (obj == null)
                {
                    Debugger.LogWarning("<<AssetLoader , SwapAssetByName>> load faile !  Path -->> " + path);
                    callback.Invoke(obj);
                    yield break;
                }

                bundle = new Asset(bundleName, obj);
                cachePool.Put(bundleName, bundle);                
            }
            callback.Invoke(bundle.Resource);
            yield break;
        }
#endif

        if (bundle != null)
        {
            if (callback != null)
                callback(bundle.Get<AssetBundle>().LoadAsset<T>(resName));
            yield break;
        }
        //异步加载
        yield return AppInter.StartCoroutine(loadBundleAndDependenices(cachePool , bundleName, (obj) =>
        { 
            if (obj == null)
            {
                Debugger.LogWarning("<<AssetLoader , SwapAssetByName>> load faile !  Path -->> " + bundleName);
                callback.Invoke(null);
                return;
            }

            Asset asset = new Asset(bundleName , obj);
            cachePool.Put(bundleName, asset);

            if (callback != null)
                callback(obj.LoadAsset<T>(resName));
        } , progress));
    }

    public IEnumerator SwapAssetByName<T>(string poolName, string resName, EAssetType assetType,Action<UnityEngine.Object> callback)
        where T : UnityEngine.Object
    {
        return SwapAssetByName<T>(poolName , resName , assetType , callback , null);
    }

#endregion
    /// <summary>
    /// 初始化预加载
    /// </summary>
    public IEnumerator InitPreLoad(Action finish)
    {
        bool isFinishMap = false;
        string assetMapName = (!isBundle ? "dev" : "") + AppPathUtils.AssetBundleMap;
        yield return AppInter.StartCoroutine(AsyncLoadAtPath(assetMapName, EAssetBaseType.Text, (obj) =>
        {
            string[] assetArr = ((string) obj).Split('\n');
            int chunkIndex = 0;

            foreach (string assetInfo in assetArr)
            {
                string asset = assetInfo.Trim();
                if(string.IsNullOrEmpty(asset)) continue;

                if (asset == AppPathUtils.AssetForMap)
                {
                    chunkIndex = 1;
                    continue;
                }

                string[] fileInfos = asset.Split(';');
                string assetType = fileInfos[0];

                string fileName = Path.GetFileName(fileInfos[1]);   //去掉目录
                fileName = fileName.Replace("." , "_" + assetType + ".");

                if (chunkIndex == 0)
                {
                    if(isBundle)
                        nameToPathMap[fileName] = fileInfos[2]; //fileInfos[2] -> Md5值
                    else
                    {
                        string srcfileName = Path.GetFileName(fileInfos[2]);
                        nameToPathMap[srcfileName + assetType] = fileInfos[2];
                    }
                }else if (chunkIndex == 1)
                {
                    string bundleName = Path.GetFileName(fileInfos[2]); //去掉目录
                    pathToBundleMap[fileName] = bundleName;
                }
            }
            isFinishMap = true;
        }));

        while (!isFinishMap)    yield return null;

        if (isBundle)
        {
            //加载assetBundleManifest
            yield return AppInter.StartCoroutine(AsyncLoadBundleAtPath("bin_none" + AppPathUtils.BundleSuffix, (bundle) =>
            {
                if (bundle != null)
                {
                    Debugger.LogWarning("<<AssetLoader , InitPreLoad>> Cant find AssetBundleManifest");
                    return;
                }
                manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }) );            
        }

        if(finish != null)  finish.Invoke();

        Debugger.Log("AssetLoader , InitPreload is finish ");
    }

    /// <summary>
    /// 获得对应资源所在的Bundle名称
    /// </summary>
    /// <param name="resName"></param>
    /// <returns>返回未加密的Bundle名称</returns>
    public string GetBundleName(string resName , EAssetType assetType)
    {
        string finalName = resName.Replace(".", "_" + assetType.ToString().ToLower() + ".");
        if (!isBundle)
        {
            //如果是开发者非Bundle模式，则直接返回最终名
            return finalName;
        }

        if (!this.pathToBundleMap.ContainsKey(finalName))
        {
            Debugger.LogWarning("<<AssetLoader , GetBundleName>> Cant find file ! name :--->> " + resName + " ,bundle : " + finalName);
            return null;
        }
        
        return pathToBundleMap[finalName];
    }
    /// <summary>
    /// 获得加密后的Bundle名称
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="assetType"></param>
    /// <returns></returns>
    public string GetBundleEncrypeName(string resName, EAssetType assetType)
    {
        string finalName = GetBundleName(resName, assetType);
        if (string.IsNullOrEmpty(finalName)) return null;

        return GetBundleEncrypeName(finalName);
    }
    /// <summary>
    /// 获得加密后的Bundle名称
    /// </summary>
    /// <param name="bundleName">未加密的Bundle文件名</param>
    public string GetBundleEncrypeName(string bundleName)
    {
        if (!nameToPathMap.ContainsKey(bundleName))
        {
            Debugger.LogWarning("<<AssetLoader , GetBundleEncrypeName>> Cant find file ! name :--->> " + bundleName);
            return null;
        }
        return nameToPathMap[bundleName];
    }

    /// <summary>
    /// 获得Bundle所有的依赖
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public string[] GetAllDependencies(string bundleName)
    {
        return manifest.GetAllDependencies(bundleName);
    }
    #endregion


    #region ----------内部类----------------
    /// <summary>
    /// 资源信息封装
    /// </summary>
    private class Asset
    {
        public string Name;
        public string Path;
        public UnityEngine.Object Resource;

        public Asset(string path, UnityEngine.Object obj)
        {
            Init(path , obj);
        }

        public void Init(string path, UnityEngine.Object obj)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Resource = obj;
        }


        public T Get<T>() where T : UnityEngine.Object
        {
            try
            {
                return (T) Resource;
            }
            catch (Exception)
            {
            }
            return default(T);
        }

        public void Unload(bool force)
        {
            AssetBundle bundle = Get<AssetBundle>();
            if (bundle != null)
            {
                bundle.Unload(force);
                return;
            }
#if UNITY_EDITOR
            GameObject.DestroyImmediate(Resource);
#else
            GameObject.Destroy(Resource);
#endif
        }
    }

#endregion
}
