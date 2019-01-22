using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using LuaInterface;
using RSG;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GEX.Resource
{
    public static class GResource
    {
        public static string RuntimeAssetsRoot = "Assets/RuntimeAssets/";

#if UNITY_EDITOR
        private static Dictionary<string, UIAtlasCache> atlasMap = new Dictionary<string, UIAtlasCache>();
#endif
        #region 异步加载Bundle资源
        /// <summary>
        /// 异步加载Prefab
        /// </summary>
        /// <param name="assetName">无后缀的资源路径</param>
        /// <returns></returns>
        public static ALoadOperation LoadBundleAsync(string assetName, string extension = ".prefab")
        {
            if (extension != ".prefab")
                Debug.LogError("no prefab load must pass GameObject value!");
            if (AppConst.AssetBundleMode)
            {
                return new LoadBundleAsync(null, assetName, extension);
            }
            string assetPath = string.Concat(RuntimeAssetsRoot, assetName, extension);

            return new LoadEditorAssetAsync(assetPath);
        }

        /// <summary>
        /// 异步加载Prefab
        /// </summary>
        /// <param name="owner">资源关联的场景gameobject（prefab类型资源可传空，其他类型资源必须有值）</param>
        /// <param name="assetName">无后缀的资源路径</param>
        /// <returns></returns>
        public static ALoadOperation LoadBundleAsync(GameObject owner, string assetName, string extension)
        {
            if (AppConst.AssetBundleMode)
            {
                return new LoadBundleAsync(owner, assetName, extension);
            }
            string assetPath = string.Concat(RuntimeAssetsRoot, assetName, extension);

            return new LoadEditorAssetAsync(assetPath);
        }

        /// <summary>
        /// 从指定缓存池（poolA）中加载资源,如果缓存池中不存在资源，就从磁盘加载
        /// <para>[[重要]]:加载完成后，需要使用指定的缓存池（poolA）进行实例（spawn）</para>
        /// </summary>
        /// <param name="cachePool">CachePoolAsync缓存池</param>
        /// <param name="assetName">无后缀的资源路径</param>
        /// <param name="owner">资源关联的场景gameobject（prefab类型资源可传空，其他类型资源必须有值）</param>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
//        public static ALoadOperation LoadCacheBundleAsync(CachePoolAsync cachePool, string assetName, string extension = ".prefab")
//        {
//            string prefabName = assetName;
//            string[] tempArr = assetName.Split('/');
//            if(tempArr.Length > 0)
//                prefabName = tempArr[tempArr.Length - 1];
//            GameObject prefab = cachePool.GetCachePrefab(prefabName);
//            if (prefab != null)
//            {
//                string assetPath = string.Concat(RuntimeAssetsRoot, assetName, extension);
//                return new LoadAssetAsync(prefab , assetPath);
//            }
//            return LoadBundleAsync(null, assetName, extension);
//        }

        /// <summary>
        /// 从指定缓存池（poolA）中加载资源,如果缓存池中不存在资源，就从磁盘加载
        /// <para>[[重要]]:加载完成后，需要使用指定的缓存池（poolA）进行实例（spawn）</para>
        /// </summary>
        /// <param name="cachePool">CachePoolAsync缓存池</param>
        /// <param name="owner">资源关联的场景gameobject（prefab类型资源可传空，其他类型资源必须有值）</param>
        /// <param name="assetName">无后缀的资源路径</param>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
//        public static ALoadOperation LoadCacheBundleAsync(CachePoolAsync cachePool, GameObject owner, string assetName, string extension = ".prefab")
//        {
//            string prefabName = assetName;
//            string[] tempArr = assetName.Split('/');
//            if (tempArr.Length > 0)
//                prefabName = tempArr[tempArr.Length - 1];
//            GameObject prefab = cachePool.GetCachePrefab(prefabName);
//            if (prefab != null)
//            {
//                string assetPath = string.Concat(RuntimeAssetsRoot, assetName, extension);
//                return new LoadAssetAsync(prefab, assetPath);
//            }
//            return LoadBundleAsync(owner, assetName, extension);
//        }

        #endregion

        #region -------------------同步加载资源-------------------

        /// <summary>
        /// 加载Resources目录资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private static T loadResources<T>(string assetName) where T : UnityEngine.Object
        {
            string resAssetPath = assetName;
            string ext = Path.GetExtension(assetName);
            if (!string.IsNullOrEmpty(ext)) resAssetPath = assetName.Replace(ext, "");
            T res = Resources.Load<T>(resAssetPath);
            if (res == null)
                Debug.LogError(string.Format("Cant find resource: ", assetName));
            return res;
        }

        /// <summary>
        /// 加载Resources目录资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static UnityEngine.Object LoadResource(string assetName, Type type)
        {
            string resAssetPath = assetName;
            string ext = Path.GetExtension(assetName);
            if (!string.IsNullOrEmpty(ext)) resAssetPath = assetName.Replace(ext, "");
            UnityEngine.Object res = Resources.Load(resAssetPath, type);
            if (res == null)
                Debug.LogError(string.Format("Cant find resource: ", assetName));
            return res;
        }

        /// <summary>
        /// 加载Resources目录文本资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static TextAsset LoadTextAssets(string assetName)
        {
            return loadResources<TextAsset>(assetName);
        }

        /// <summary>
        /// 加载Resources目录Sprite资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static Sprite LoadSpriteAssets(string assetName)
        {
            return loadResources<Sprite>(assetName);
        }

        /// <summary>
        /// 加载Resources目录GameObject资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static GameObject LoadGameObjectAssets(string assetName)
        {
            return loadResources<GameObject>(assetName);
        }

#region Bundle加载

        /// <summary>
        /// 加载Prefab资源, Bundle类型资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static GameObject LoadPrefabBundle(string assetName)
        {
            GameObject go = null;
            if (AppConst.AssetBundleMode)
                go = AssetBundleManager.Instance.LoadPrefab(assetName);
#if UNITY_EDITOR
            else
                go = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}{1}.prefab", RuntimeAssetsRoot, assetName));
#endif
            return go;
        }

        /// <summary>
        /// 加载Prefab资源, Bundle类型资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
//        public static GameObject LoadCachePrefabBundle(CachePoolAsync cachePool, string assetName)
//        {
//            string prefabName = assetName;
//            string[] tempArr = assetName.Split('/');
//            if (tempArr.Length > 0)
//                prefabName = tempArr[tempArr.Length - 1];
//            GameObject prefab = cachePool.GetCachePrefab(assetName);
//            if (prefab != null)
//            {
//                return prefab;
//            }
//            else return LoadPrefabBundle(assetName);
//        }

        /// <summary>
        /// 加载二进制数据, Bundle类型资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static byte[] LoadBytesBundle(string assetName)
        {
            if (AppConst.AssetBundleMode)
                return AssetBundleManager.Instance.LoadBytes(assetName);
#if UNITY_EDITOR
            string extension = Path.GetExtension(assetName);
            if (!string.IsNullOrEmpty(extension))
                assetName = assetName.Replace(extension, "");
            TextAsset textAss = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format("{0}{1}.bytes", RuntimeAssetsRoot, assetName));
            return textAss.bytes;
#endif
            return null;
        }

        /// <summary>
        /// 加载贴图, Bundle类型资源
        /// </summary>
        /// <param name="assetPath">相对路径（带后缀）</param>
        /// <returns></returns>
        public static Texture LoadTextureBundle(GameObject owner, string assetPath)
        {
            if (AppConst.AssetBundleMode)
                return AssetBundleManager.Instance.LoadTexture(owner, assetPath);
#if UNITY_EDITOR
            string fullPath = string.Concat(RuntimeAssetsRoot, assetPath);
            if (!File.Exists(fullPath))
            {
                Debug.LogError(string.Format("无法找到指定的图片资源！ 路径：{0}", assetPath));
                return null;
            }
            Texture textureAss = AssetDatabase.LoadAssetAtPath<Texture>(fullPath);
            return textureAss;
#endif
            return null;
        }
        /// <summary>
        /// 加载Scriptable类型的.asset文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        [NoToLua]
        public static T LoadAssetBundle<T>(string assetName) where T : ScriptableObject
        {
            if (AppConst.AssetBundleMode)
                return AssetBundleManager.Instance.LoadAssets<T>(assetName);
#if UNITY_EDITOR
            string extension = Path.GetExtension(assetName);
            if (!string.IsNullOrEmpty(extension))
                assetName = assetName.Replace(extension, "");
            T obj = AssetDatabase.LoadAssetAtPath<T>(string.Concat(RuntimeAssetsRoot ,assetName , ".asset"));
            return obj;
#endif
            return null;
        }

        /// <summary>
        /// 从Atlas集合中加载Sprite对象
        /// </summary>
        /// <param name="spriteName">精灵图片名称</param>
        /// <param name="atlasName">图集名称:Atlas/common/common-0</param>
        /// <returns></returns>
        public static Sprite LoadSpriteFromAtlasBundle(GameObject owener, string spriteName, string atlasName)
        {
            if (AppConst.AssetBundleMode)
                return AssetBundleManager.Instance.LoadSprite(owener, spriteName, atlasName);
#if UNITY_EDITOR
            using (zstring.Block())
            {
                if (atlasName.Contains("-"))
                    Debug.LogErrorFormat("atlas name contains \'-\', please check your code: [sprite]{0}, [atlas]{1}", spriteName, atlasName);
                zstring dir = Path.GetDirectoryName(atlasName);

                zstring extension = Path.GetExtension(spriteName);
                if (!zstring.IsNullOrEmpty(extension))
                    spriteName = spriteName.Replace(extension, "");

                UIAtlasCache atlas = null;
                if (!atlasMap.TryGetValue(dir, out atlas))
                {
                    zstring rootDir = RuntimeAssetsRoot + dir;
                    string[] textureGuids = AssetDatabase.FindAssets("t:Texture", new string[] { rootDir });
                    List<Sprite> sprites = new List<Sprite>();
                    for (int i = 0; i < textureGuids.Length; i++)
                    {
                        zstring relativePath = AssetDatabase.GUIDToAssetPath(textureGuids[i]);
                        zstring childDir = Path.GetDirectoryName(relativePath);
                        if (!childDir.Equals(rootDir)) continue; //只遍历顶层目录
                        var assets = AssetDatabase.LoadAllAssetsAtPath(relativePath);

                        foreach (var asset in assets)
                        {
                            if (asset is Sprite)
                                sprites.Add(asset as Sprite);
                        }
                    }
                    atlas = new UIAtlasCache(sprites.ToArray());
                    atlasMap[dir] = atlas;
                }

                return atlas.GetSprite(spriteName);
            }   
#endif
            return null;
        }

#endregion

#endregion


#region -------------------加载音频数据-------------------

        public static IPromise<AudioClip> LoadAudioClipBundleAsync(GameObject owner, string name, string extension)
        {
            if (AppConst.AssetBundleMode)
            {
                return AssetBundleManager.Instance.LoadAudioClipBundleAsync(owner, name, extension);
            }

            AudioClip clip = null;
#if UNITY_EDITOR
            string path = string.Format("{0}{1}.{2}",RuntimeAssetsRoot, name, extension);
            clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
#endif
            return Promise<AudioClip>.Resolved(clip);
        }
#endregion
    }
}