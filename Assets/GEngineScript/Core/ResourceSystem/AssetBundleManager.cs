using UnityEngine;
using System.Collections;
using LuaFramework;
using System.IO;
using System.Collections.Generic;
using System;
using GEX.Utility;
using RSG;

namespace GEX.Resource
{
    public class BundleInfo
    {
        public AssetBundle bundle
        {
            get
            {
                refCount++;
                lastTimeVisited = Time.realtimeSinceStartup;
                AssetBundleManager abMgr = AssetBundleManager.Instance;
                var deps = abMgr.GetAllDependencies(_bundleName);
                for (int i = 0; i < deps.Count; ++i)
                {
                    if (deps[i].Contains("tmp/")) continue;
                    BundleInfo bundleInfo;
                    if (abMgr.GetBundleInfo(deps[i], out bundleInfo))
                    {
                        bundleInfo.refCount++;
                    }
                    else if (abMgr.bundleLoadingQueue.Contains(deps[i]))
                    {
                        continue;
                    }
                    else
                    {
                        var temp = AssetBundle.LoadFromFile(Util.DataPath + deps[i]);
                        abMgr.AddBundleToLoaded(deps[i], temp);
                        //abMgr.bundleLoaded.Add(, new BundleInfo(temp, deps[i]));
                    }
                }
                return _bundle;
            }
            private set
            {
                _bundle = value;
            }
        }

        private AssetBundle _bundle;
        public int refCount
        {
            get { return _refCount; }
            set
            {
                if (value > _refCount) lastTimeVisited = Time.realtimeSinceStartup;
                _refCount = value;
            }
        }

        private int _refCount;

        public float timeCreated { get; private set; }
        public float lastTimeVisited { get; private set; }
        public string _bundleName { get; private set; }

        public BundleInfo(AssetBundle bundle, string name)
        {
            this._bundle = bundle;
            this._refCount = 1;
            this._bundleName = name;
            timeCreated = Time.realtimeSinceStartup;
            lastTimeVisited = Time.realtimeSinceStartup;
        }

        public bool Unload()
        {
            AssetMap map;
            if (AssetBundleManager.Instance.bundleLoadTypeMap.TryGetValue(_bundleName, out map))
                if (map.type == AssetMap.LoadType.Preload) return false;

            using (zstring.Block())
                Debugger.Log(zstring.Format("<color=green>assetbundle unload: {0}</color>", _bundleName));
            if (_bundle != null) _bundle.Unload(true);
            _bundle = null;
            return true;
        }
    }

    public struct AssetMap
    {
        public enum LoadType
        {
            None,
            Preload,
            Limited,
        }
        public string name;
        /// <summary>
        /// asset类型: 0 普通类型; 1 预加载; 2 限制加载（总数量限制）
        /// </summary>
        public LoadType type;
    }

    public sealed class AssetBundleManager : Singleton<AssetBundleManager>
    {
        public AssetBundleManifest allBundleManifest { get; private set; }

        /// <summary>
        /// 正在加载的bundle资源
        /// </summary>
        public HashSet<string> bundleLoadingQueue = new HashSet<string>();
        /// <summary>
        /// 已经加载的bundle资源
        /// </summary>
        public Dictionary<string, BundleInfo> bundleCache = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// asset资源map映射表
        /// </summary>
        public Dictionary<string, string> assetbundleMap = new Dictionary<string, string>();

        private Dictionary<string , UIAtlasCache> atlasMap = new Dictionary<string, UIAtlasCache>();
         
        private Dictionary<string, int> assetIntanceCounts = new Dictionary<string, int>();

        private Queue<BundleInfo> unloadQueue = new Queue<BundleInfo>();

        public delegate void OnProgressChanged(float value);

        public OnProgressChanged onProgressChange;

        public Dictionary<string, AssetMap> bundleLoadTypeMap = new Dictionary<string, AssetMap>();

        /// <summary>
        /// 已经加载的限制类型bundle资源(refcount=0需立即释放)
        /// </summary>
        private Dictionary<string, BundleInfo> limitedBundleCache = new Dictionary<string, BundleInfo>();

        public UnityEngine.Object[] shaderAssets;

        public Font font;

        private AssetBundleCreateRequest preloadAbcr = null;

        private int curIndex = 0;

        const float MAXDELAY_CLEAR_TIME = 10 * 60;
        private float delayClearTime;

        private float delayUnloadTime;

        //private AssetBundle sceneDependency = null;

        private Dictionary<string, List<string>> dependenciesCache = new Dictionary<string, List<string>>();

        new void Awake()
        {
            delayClearTime = MAXDELAY_CLEAR_TIME;
        }

        public void AssetBundleInit(Action callback)
        {
            LoadAssetbundleMap();
            LoadManifest();
            StartCoroutine(PreloadAsset(callback));
            //limitedBundleCache.SetDestroyStrategy(new BundleDestoryStrategy(AssetBundleManager.Instance));
        }

        private void LoadAssetbundleMap()
        {
            var content = File.ReadAllBytes(string.Format("{0}/bundlemap.ab", Util.DataPath));
            var decryptoStrs = Encoding.GetString(Crypto.Decode(content)).Split('\n');
            for (int i = 0; i < decryptoStrs.Length; ++i)
            {
                if (!string.IsNullOrEmpty(decryptoStrs[i]))
                {
                    var temp = decryptoStrs[i].Split('|');
                    Debug.Assert(temp.Length == 3);
                    if (assetbundleMap.ContainsKey(temp[0]))
                    {
                        throw new ArgumentException(string.Format("{0} is already exists", temp[0]));
                    }
                    if (temp[0].CustomStartsWith("atlas/"))
                        assetbundleMap[temp[0].Split('-')[0]] = temp[1];
                    else
                        assetbundleMap[temp[0]] = temp[1];
                    if (!bundleLoadTypeMap.ContainsKey(temp[1]))
                    {
                        AssetMap map = new AssetMap();
                        map.name = temp[1];
                        map.type = (AssetMap.LoadType)(Convert.ToInt32(temp[2].Trim()));
                        bundleLoadTypeMap[map.name] = map;
                    }
                    //if (temp[2].Trim() == "1" && !preloadAssets.Contains(temp[1]))
                    //    preloadAssets.Add(temp[1]);
                }
            }
        }

        private void LoadManifest()
        {
            string path = string.Format("{0}{1}.ab", Util.DataPath, LuaConst.osDir.ToLower());
            if (!File.Exists(path))
            {
                Debug.LogError(string.Format("no manifest file exists in {0}", path));
                return;
            }
            var allBundle = AssetBundle.LoadFromFile(path);
            if (allBundle)
            {
                allBundleManifest = allBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                allBundle.Unload(false);
                var bundles = allBundleManifest.GetAllAssetBundles();
                
                for (int i = 0; i < bundles.Length; ++i)
                {
                    AssetMap map;
                    if (bundleLoadTypeMap.TryGetValue(bundles[i], out map))
                    {
                        var deps = GetAllDependencies(bundles[i]);
                        for (int k = 0; k < deps.Count; ++k)
                        {
                            if (deps[k].Contains("tmp/")) continue;
                            if (map.type == AssetMap.LoadType.Preload && !IsBundleLoaded(deps[k]))
                            {
                                AssetMap depMap;
                                if (!bundleLoadTypeMap.TryGetValue(deps[k], out depMap))
                                {
                                    depMap = new AssetMap();
                                    depMap.name = deps[k];
                                    depMap.type = AssetMap.LoadType.Preload;
                                    bundleLoadTypeMap[deps[k]] = depMap;
                                }
                                else if(depMap.type != AssetMap.LoadType.Preload)
                                {
                                    AssetMap tmp = new AssetMap();
                                    tmp.name = depMap.name;
                                    tmp.type = AssetMap.LoadType.Preload;
                                    bundleLoadTypeMap[deps[k]] = tmp;
                                }
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator PreloadAsset(Action callback)
        {
            foreach (var map in bundleLoadTypeMap.Values)
            {
                if (map.type == AssetMap.LoadType.Preload)
                {
                    string assetName = map.name;
                    string path = string.Format("{0}{1}", Util.DataPath, assetName);
                    if (assetName.CustomEndsWith(".ab") && File.Exists(path))
                    {
                        if (!IsBundleLoaded(assetName))
                        {
                            preloadAbcr = AssetBundle.LoadFromFileAsync(path);
                            yield return preloadAbcr;
                            AddBundleToLoaded(assetName, preloadAbcr.assetBundle);
                        }
                    }
                }
                curIndex++;
                if (curIndex % 20 == 0)
                    yield return null;
            }
            preloadAbcr = null;
            if (callback != null) callback();
        }

        public Shader GetShader(string name)
        {
            for (int i = 0; i < shaderAssets.Length; ++i)
            {
                if (shaderAssets[i].name.Equals(name))
                    return shaderAssets[i] as Shader;
            }
            return null;
        }

        public IEnumerator PreloadShaderAndFont()
        {
            if (!AppConst.AssetBundleMode) yield break;
            string dataPath = Util.DataPath;  //数据目录
            string resPath = AppPathUtils.StreamingPathFormat(LuaConst.osDir); //游戏包资源目录
            //加载shader.ab
            string shaderABName = "shader.ab";
            string shaderFile = Path.Combine(resPath, shaderABName);
            string outFile = Path.Combine(dataPath, shaderABName);
            AssetMap map = new AssetMap();
            map.name = shaderABName;
            map.type = AssetMap.LoadType.Preload;
            bundleLoadTypeMap.Add(shaderABName, map);
            yield return preload(shaderFile, outFile, shaderABName);

            //加载font.ab
            string fontABName = "font.ab";
            string fontFile = Path.Combine(resPath, fontABName);
            outFile = Path.Combine(dataPath, fontABName);
            map = new AssetMap();
            map.name = fontABName;
            map.type = AssetMap.LoadType.Preload;
            bundleLoadTypeMap.Add(fontABName, map);
            yield return preload(fontFile, outFile, fontABName);

            font = GetBundleFromLoaded(fontABName).LoadAsset("FZCuYuan") as Font;
            shaderAssets = GetBundleFromLoaded(shaderABName).LoadAllAssets();
            Shader.WarmupAllShaders();
        }

        IEnumerator preload(string url, string outFile, string key)
        {
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                var abcr = AssetBundle.LoadFromMemoryAsync(www.bytes);
                yield return abcr;
                AddBundleToLoaded(key, abcr.assetBundle);
                //bundleCache.Add(key, new BundleInfo(abcr.assetBundle, key));
                AssetMap map;
                if (bundleLoadTypeMap.TryGetValue(key, out map))
                {
                    map.type = AssetMap.LoadType.Preload;
                }
                //preloadAssets.Add(key);
            }
            else
            {
                Debug.LogError(www.error);
                yield break;
            }
            www.Dispose();
        }

        void Update()
        {
            if (preloadAbcr != null)
            {
                if (onProgressChange != null)
                    onProgressChange((float)(curIndex + 1 + preloadAbcr.progress) / (float)bundleLoadTypeMap.Count);
            }
            delayClearTime -= Time.deltaTime;
            //if (Input.GetMouseButton(0) || Input.touchCount > 0)
            //{
            //    delayClearTime = MAXDELAY_CLEAR_TIME;
            //}
            if (delayClearTime <= 0)
            {
                delayClearTime = MAXDELAY_CLEAR_TIME;
                Util.ClearMemory();
            }
        }

        private void FixedUpdate()
        {
#if UNITY_IOS
            float realTime = Time.realtimeSinceStartup;
            if (unloadQueue.Count > 0)
            {
                BundleInfo info = unloadQueue.Dequeue();
                if (info.refCount <= 0)
                {
                    info.Unload();
                    limitedBundleCache.Remove(info._bundleName);
                    UnityEngine.Resources.UnloadUnusedAssets();
                }
            }
            if (realTime - delayUnloadTime >= 10)
            {
                foreach (var val in limitedBundleCache.Values)
                {
                    if (val.refCount <= 0)
                        unloadQueue.Enqueue(val);
                }
                delayUnloadTime = realTime;
            }
#endif
        }

        public IEnumerator LoadSceneDependency(string sceneBundleName)
        {
            var deps = GetAllDependencies(sceneBundleName);
            for (int i = 0; i < deps.Count; ++i)
            {
                if (deps[i].Contains("tmp/")) continue;
                while (bundleLoadingQueue.Contains(deps[i]))
                    yield return Yielders.EndOfFrame;
                BundleInfo bundleInfo;
                if (GetBundleInfo(deps[i], out bundleInfo))
                {
                    bundleInfo.refCount++;
                }
                else
                {
                    using (zstring.Block())
                    {
                        zstring depPath = Util.DataPath + deps[i];
                        if (File.Exists(depPath))
                        {
                            bundleLoadingQueue.Add(deps[i]);
                            var abcr = AssetBundle.LoadFromFileAsync(depPath);
                            yield return abcr;
                            bundleLoadingQueue.Remove(deps[i]);
                            AddBundleToLoaded(deps[i], abcr.assetBundle);
                            //bundleLoaded.Add(deps[i], new BundleInfo(abcr.assetBundle, deps[i]));
                        }
                        else
                        {
                            Debug.LogWarning(zstring.Format("{0} not exists", depPath));
                        }
                    }   
                }
            }
        }

        public void UnloadSceneDependency()
        {
            //if (sceneDependency != null)
            //    sceneDependency.Unload(false);
            //sceneDependency = null;
        }


        public AssetBundle TryGetBundle(string bundleName)
        {
            AssetBundle bundle = null;
            try
            {
                using (zstring.Block())
                {
                    zstring key = bundleName;
                    bundle = GetBundleFromLoaded(key);
                    if (bundle == null)
                    {
                        if (bundleLoadingQueue.Contains(key))
                        {
                            Debug.LogWarning(zstring.Format("资源正在异步加载中: {0}", key));
                            return null;
                        }
                        else
                        {
                            var deps = GetAllDependencies(key);
                            for (int i = 0; i < deps.Count; ++i)
                            {
                                if (deps[i].Contains("tmp/")) continue;
                                var depBundle = GetBundleFromLoaded(deps[i]);
                                if (depBundle == null)
                                {
                                    if (bundleLoadingQueue.Contains(deps[i]))
                                    {
                                        Debug.LogWarning(zstring.Format("资源正在异步加载中: {0}", key));
                                        return null;
                                    }
                                    else
                                    {

                                        zstring depPath = Util.DataPath + deps[i];
                                        if (File.Exists(depPath))
                                        {
                                            var temp = AssetBundle.LoadFromFile(depPath);
                                            AddBundleToLoaded(deps[i], temp);
                                            //bundleLoaded.Add(deps[i], new BundleInfo(temp, deps[i]));
                                        }
                                        else
                                        {
                                            throw new FileNotFoundException(zstring.Format("File {0} not exists", depPath));
                                        }
                                    }
                                }
                            }
                            zstring path = Util.DataPath + key;
                            if (File.Exists(path))
                            {
                                if (!IsBundleLoaded(key))
                                {
                                    bundle = AssetBundle.LoadFromFile(path);
                                    AddBundleToLoaded(key, bundle);
                                    //bundleLoaded.Add(key, new BundleInfo(bundle, key));
                                }
                                else
                                {
                                    throw new ArgumentException(zstring.Format("key {0} is already exist in bundleLoaded", key));
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException(zstring.Format("File {0} not exists", path));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("Get Assetbundle Exception, Name: {0}", bundleName));
                Debug.LogException(e);
                return null;
            }
            return bundle;
        }

        public AssetBundle TryGetBundleByFile(string fileName)
        {
            AssetBundle bundle = null;
            
            string key;
            if (!assetbundleMap.TryGetValue(fileName, out key))
            {
                Debug.LogWarning(string.Format("{0} has no assetbundle resource", fileName));
            }
            else
            {
                return TryGetBundle(key);
            }

            return bundle;
        }

        public IEnumerator TryToGetBundleAsync(string fileName, Action<AssetBundle> callback)
        {
            using (zstring.Block())
            {
                AssetBundle bundle = null;
                string key;
                if (!assetbundleMap.TryGetValue(fileName, out key))
                {
                    Debug.LogError(zstring.Format("{0} has no assetbundle resource", fileName));
                }
                else
                {
                    while (bundleLoadingQueue.Contains(key))
                        yield return Yielders.EndOfFrame;
                    bundle = GetBundleFromLoaded(key);
                    if (bundle == null)
                    {
                        bundleLoadingQueue.Add(key);
                        var deps = GetAllDependencies(key);
                        for (int i = 0; i < deps.Count; ++i)
                        {
                            if (string.IsNullOrEmpty(deps[i]) || deps[i].Contains("tmp/")) continue;

                            while (bundleLoadingQueue.Contains(deps[i]))
                                yield return Yielders.EndOfFrame;
                            var depBundle = GetBundleFromLoaded(deps[i]);
                            if (depBundle == null)
                            {
                                zstring depPath = Util.DataPath + deps[i];
                                if (File.Exists(depPath))
                                {
                                    bundleLoadingQueue.Add(deps[i]);
                                    var abcr = AssetBundle.LoadFromFileAsync(depPath);
                                    yield return abcr;
                                    bundleLoadingQueue.Remove(deps[i]);
                                    AddBundleToLoaded(deps[i], abcr.assetBundle);
                                    //bundleLoaded.Add(deps[i], new BundleInfo(abcr.assetBundle, deps[i]));
                                }
                                else
                                {
                                    Debug.LogWarning(zstring.Format("{0} not exists", depPath));
                                }
                            }
                        }
                        zstring path = Util.DataPath + key;
                        if (File.Exists(path))
                        {
                            var mainAbcr = AssetBundle.LoadFromFileAsync(path);
                            yield return mainAbcr;
                            bundle = mainAbcr.assetBundle;
                            AddBundleToLoaded(key, bundle);
                            //if (!bundleLoaded.ContainsKey(key))
                            //    bundleLoaded.Add(key, new BundleInfo(bundle, key));
                        }
                        else
                        {
                            Debug.LogWarning(zstring.Format("{0} not exists", path));
                        }
                        bundleLoadingQueue.Remove(key);
                    }
                }
                if (callback != null) callback(bundle);
            }  
        }

        public List<string> GetAllDependencies(string key)
        {
            List<string> result;
            try
            {
                if (!dependenciesCache.TryGetValue(key, out result))
                {
                    result = new List<string>();
                    result.AddRange(allBundleManifest.GetAllDependencies(key));
                    dependenciesCache.Add(key, result);
                }
            }
            catch
            {
                result = new List<string>();
            }
            return result;
        }

        public bool GetBundleInfo(string bundleName, out BundleInfo bundleInfo)
        {
            bundleInfo = null;
            if (!bundleCache.TryGetValue(bundleName, out bundleInfo))
                return limitedBundleCache.TryGetValue(bundleName, out bundleInfo);
            return true;
        }

        AssetBundle GetBundleFromLoaded(string bundleName)
        {
            BundleInfo bundleInfo;
            AssetBundle bundle = null;
            if (bundleCache.TryGetValue(bundleName, out bundleInfo))
            {
                bundle = bundleInfo.bundle;
            }
            else if (limitedBundleCache.TryGetValue(bundleName, out bundleInfo))
            {
                bundle = bundleInfo.bundle;
            }
            return bundle;
        }

        public void AddBundleToLoaded(string bundleName, AssetBundle bundle)
        {
            AssetMap map;
            if (bundleLoadTypeMap.TryGetValue(bundleName, out map))
            {
                if (map.type == AssetMap.LoadType.Limited)
                {
                    if (!limitedBundleCache.ContainsKey(bundleName))
                        limitedBundleCache.Add(bundleName, new BundleInfo(bundle, bundleName));
                }
                else
                {
                    if (!bundleCache.ContainsKey(bundleName))
                        bundleCache.Add(bundleName, new BundleInfo(bundle, bundleName));
                }
            }
            else
            {
                map = new AssetMap();
                map.name = bundleName;
                map.type = AssetMap.LoadType.None;
                bundleLoadTypeMap.Add(bundleName, map);
                if (!bundleCache.ContainsKey(bundleName))
                    bundleCache.Add(bundleName, new BundleInfo(bundle, bundleName));
            }
        }

        bool IsBundleLoaded(string bundleName)
        {
            return bundleCache.ContainsKey(bundleName) || limitedBundleCache.ContainsKey(bundleName);
        }

        AssetMap.LoadType GetAssetLoadType(string assetName)
        {
            if (assetbundleMap.ContainsKey(assetName))
            {
                var key = assetbundleMap[assetName];
                AssetMap map;
                if (bundleLoadTypeMap.TryGetValue(key, out map))
                    return map.type;
            }
            return AssetMap.LoadType.None;
        }

        void RemoveBundle(string bundleName)
        {
            if (!bundleCache.ContainsKey(bundleName))
                bundleCache.Remove(bundleName);
            else if (!limitedBundleCache.ContainsKey(bundleName))
                limitedBundleCache.Remove(bundleName);
        }

        public IPromise<T> LoadAssetAsync<T>(string assetName, string extension, GameObject owner)
            where T : UnityEngine.Object
        {
            return new Promise<T>((resolved, reject) =>
            {
                new Promise<AssetBundle>((s, j) =>
                {
                    StartCoroutine(TryToGetBundleAsync(assetName, s));
                }).Then((bundle) =>
                {
                    StartCoroutine(_loadBundleAsset(owner, assetName, extension, bundle, resolved));
                }).Catch((e) =>
                {
                    reject(e);
                    Debug.LogException(e);
                });
            });
        }

        public IPromise<GameObject> LoadPrefabAsync(string assetName)
        {
           return LoadAssetAsync<GameObject>(assetName, "prefab", null).Then((go) =>
           {
               using (zstring.Block())
               {
                   if (go == null)
                   {
                       zstring assetPath = zstring.Concat(GResource.RuntimeAssetsRoot , assetName , ".prefab");
                       throw new Exception(zstring.Format("load prefab error: {0}", assetPath));
                   }
#if UNITY_EDITOR
                   Util.ResetShader(go);
#endif
                   string bundleName;
                   if (assetbundleMap.TryGetValue(assetName, out bundleName))
                   {
                       go.transform.GetOrAddComponent<AssetWatcher>().bundleName = bundleName;
                   }
                   else
                   {
                       Debug.LogErrorFormat("can't find {0} in bundleMap", assetName);
                   }
               }
                   
           }).Catch(e =>
           {
               Debug.LogException(e);
           });
        }

#region -------------同步加载----------------------
        /// <summary>
        /// 同步加载Prefab文件
        /// </summary>
        /// <returns></returns>
        public GameObject LoadPrefab(string name)
        {
            using (zstring.Block())
            {
                zstring zname = name.ToLower();
                zstring assetName = zstring.Format("{0}.prefab", zname);
                GameObject obj = LoadAssets<GameObject>(assetName);
                if (obj != null)
                {
#if UNITY_EDITOR
                    Util.ResetShader(obj);
#endif
                    string bundleName;
                    if (assetbundleMap.TryGetValue(zname, out bundleName))
                    {
                        obj.transform.GetOrAddComponent<AssetWatcher>().bundleName = bundleName;
                    }
                    else
                    {
                        Debug.LogErrorFormat("can't find {0} in bundleMap", zname);
                    }
                }
                else
                {
                    Debug.LogWarning(string.Format("load prefab error: {0}, {1}", zname, assetName));
                }
                return obj;
            }
        }

        public Sprite LoadSprite(GameObject owner, string spriteName, string atlasName)
        {
            using (zstring.Block())
            {
                zstring assetName = atlasName.ToLower();
                zstring ext = Path.GetExtension(assetName);
                if (!string.IsNullOrEmpty(ext))
                    assetName = assetName.Replace(ext, "");

                string bundleRelativePath = assetName;
                if (!assetbundleMap.TryGetValue(assetName, out bundleRelativePath))
                {
                    Debug.LogWarning(zstring.Format("{0} has no assetbundle resource", assetName));
                    return null;
                }

                UIAtlasCache atlasCache = null;
                if (!atlasMap.TryGetValue(bundleRelativePath, out atlasCache))
                {
                    AssetBundle bundle = TryGetBundleByFile(assetName);
                    if (bundle == null) return null;

                    var assets = bundle.LoadAllAssets<Sprite>();
                    if (assets == null)
                        Debug.LogWarning(zstring.Format("Cant find sprite: {0} in bundle {1}", spriteName, bundleRelativePath));

                    atlasCache = new UIAtlasCache(assets);
                    atlasMap[bundleRelativePath] = atlasCache;
                }

                Sprite sprite = atlasCache.GetSprite(spriteName);
                TextureWatcher watcher = owner.transform.GetOrAddComponent<TextureWatcher>();
                watcher.AddBundleName(bundleRelativePath);

                return sprite;
            }
        }

        public Texture LoadTexture(GameObject owner, string assetPath)
        {
            using (zstring.Block())
            {
                zstring assetName = assetPath.ToLower();
                zstring ext = Path.GetExtension(assetName);
                if (!zstring.IsNullOrEmpty(ext))
                    assetName = assetName.Replace(ext, "");

                AssetBundle bundle = TryGetBundleByFile(assetName);
                if (bundle == null) return null;

                TextureWatcher watcher = owner.transform.GetOrAddComponent<TextureWatcher>();
                string bundleName;
                if (assetbundleMap.TryGetValue(assetName, out bundleName))
                    watcher.AddBundleName(bundleName);

                string assetRoot = GResource.RuntimeAssetsRoot.ToLower();
                if (!assetName.StartsWith(assetRoot))
                    assetName = zstring.Concat(assetRoot, assetPath);

                var asset = bundle.LoadAsset(assetName) as Texture;
                if (asset == null)
                    Debug.LogWarning(zstring.Format("Cant find ab: {0}", assetName));
                return asset;
            }
        }

        /// <summary>
        /// 加载字节数组
        /// </summary>
        /// <param name="path">无.bytes后缀的路径</param>
        /// <returns></returns>
        public byte[] LoadBytes(string path)
        {
            using (zstring.Block())
            {
                zstring zpath = path.ToLower();
                TextAsset textAss = LoadAssets<TextAsset>(zstring.Format("{0}.bytes", zpath));
                if (textAss != null)
                    return textAss.bytes;
                return null;
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadAssets<T>(string path) where T : UnityEngine.Object
        {
            using (zstring.Block())
            {
                zstring assetName = path.ToLower();
                zstring ext = Path.GetExtension(assetName);
                if (!zstring.IsNullOrEmpty(ext))
                    assetName = assetName.Replace(ext, "");

                AssetBundle bundle = TryGetBundleByFile(assetName);
                if (bundle == null) return null;

                string assetRoot = GResource.RuntimeAssetsRoot.ToLower();
                if (!assetName.StartsWith(assetRoot))
                    assetName = zstring.Concat(assetRoot , path);

                var asset = bundle.LoadAsset(assetName) as T;
                if (asset == null)
                    Debug.LogWarning(zstring.Format("Cant find ab: {0}", assetName));
                return asset;
            }   
        }
#endregion

        public IPromise<AudioClip> LoadAudioClipBundleAsync(GameObject owner, string assetName, string extension)
        {
            assetName = assetName.ToLower();
            return LoadAssetAsync<AudioClip>(assetName, extension, owner);
        }

        private IEnumerator _loadBundleAsset<T>(GameObject owner, string goName, string extension, AssetBundle bundle, Action<T> callback)
            where T : UnityEngine.Object
        {
            if (bundle != null)
            {
                using (zstring.Block())
                {
                    zstring assetName = zstring.Format("{0}{1}.{2}", GResource.RuntimeAssetsRoot.ToLower() , goName, extension.TrimStart('.'));
                    var abcr = bundle.LoadAssetAsync(assetName);
                    yield return abcr;
                    var mainAsset = (T)abcr.asset;
                    if (extension == ".asset")
                    {
                        TextAssetWatcher watcher = owner.transform.GetOrAddComponent<TextAssetWatcher>();
                        string bundleName;
                        if (assetbundleMap.TryGetValue(goName, out bundleName))
                        {
                            watcher.AddBundleName(bundleName);
                        }
                        else Debug.LogError("can't find " + goName + " in assetbundleMap");
                    }
                    callback(mainAsset);
                }
            }
            else
                callback(default(T));
        }

        public int GetRefCount(string assetName)
        {
            BundleInfo info;
            if (GetBundleInfo(assetName, out info))
                return info.refCount;
            return -1;
        }

        #region LoadAsset & UnloadAsset

        public void LoadAsset(string bundleName, GameObject go)
        {
            if (!assetIntanceCounts.ContainsKey(bundleName))
            {
                assetIntanceCounts.Add(bundleName, 1);
            }
            else
            {
                assetIntanceCounts[bundleName]++;
            }
        }

        public void UnloadAsset(string bundleName, GameObject go)
        {
            bool unload = false;
            if (assetIntanceCounts.ContainsKey(bundleName))
            {
                assetIntanceCounts[bundleName]--;
                unload = assetIntanceCounts[bundleName] <= 0;
            }
            else
                unload = true;

            if (unload)
            {
                UnloadDependency(bundleName);
                BundleInfo info;
                if (GetBundleInfo(bundleName, out info)) info.refCount--;

                assetIntanceCounts.Remove(bundleName);
            }
        }

        public void LoadAsset(string assetName)
        {
            UIAtlasCache cache;
            if (!atlasMap.TryGetValue(assetName, out cache))
            {
                BundleInfo info;
                if (GetBundleInfo(assetName, out info))
                {
                    LoadDependency(assetName);
                    info.refCount++;
                }
                else
                {
                    Debug.LogError("load asset error! can't find asset in cache: " + assetName);
                }
                return;
            }
            cache.Load();
        }

        public void UnloadAsset(string name)
        {
            UIAtlasCache cache;
            if (!atlasMap.TryGetValue(name, out cache))
            {
                BundleInfo info;
                if (GetBundleInfo(name, out info))
                {
                    UnloadDependency(name);
                    info.refCount--;
                }
                else
                {
                    Debug.LogError("unload asset error! can't find asset in cache: " + name);
                }
                return;
            }
            if (cache.Unload())
            {
                BundleInfo info;
                if (GetBundleInfo(name, out info))
                {
                    UnloadDependency(name);
                    info.refCount--;
                }
            }
        }

        private void UnloadDependency(string name)
        {
            var deps = GetAllDependencies(name);
            for (int i = 0; i < deps.Count; ++i)
            {
                BundleInfo info;
                if (deps[i].Contains("tmp/")) continue;
                if (GetBundleInfo(deps[i], out info))
                    info.refCount--;
            }
        }

        private void LoadDependency(string name)
        {
            var deps = GetAllDependencies(name);
            for (int i = 0; i < deps.Count; ++i)
            {
                BundleInfo info;
                if (deps[i].Contains("tmp/")) continue;
                if (GetBundleInfo(deps[i], out info))
                    info.refCount++;
            }
        }

        public void Clear()
        {
            delayClearTime = MAXDELAY_CLEAR_TIME;
            var keys = new List<string>();
            foreach (var key in bundleCache.Keys)
            {
                if (bundleCache[key].refCount <= 0)
                {
                    if (bundleCache[key].Unload())
                        keys.Add(key);
                }
            }
            for (int i = 0; i < keys.Count; ++i)
            {
                if (atlasMap.ContainsKey(keys[i]))
                    atlasMap.Remove(keys[i]);
                bundleCache.Remove(keys[i]);
            }
            foreach (var key in limitedBundleCache.Keys)
            {
                limitedBundleCache[key].Unload();
            }
            limitedBundleCache.Clear();
            Util.ClearMemory();
        }
        #endregion
    }
}