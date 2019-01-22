using System.Collections;
using System.IO;
using LuaFramework;
using LuaInterface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GEX.Resource
{
    /// <summary>
    /// 场景资源加载器
    /// </summary>
    public class LoadSceneAsync : ALoadOperation
    {

        private AsyncOperation loading;

        private LoadSceneMode loadSceneMode;

        private AssetBundleCreateRequest abcr;

        public AsyncOperation AsyncSceneLoader
        {
            get { return loading; }
        }

        public LoadSceneAsync(string path)
        {
            this.assetPath = path;
            loadSceneMode = LoadSceneMode.Single;
        }

        public LoadSceneAsync(string path, LoadSceneMode loadMode)
        {
            this.assetPath = path;
            this.loadSceneMode = loadMode;
        }

        public override void OnLoad()
        {
            loading = SceneManager.LoadSceneAsync(this.assetPath, loadSceneMode);
            loading.allowSceneActivation = false;
        }


        private IEnumerator loadAsync()
        {
            if (AppConst.AssetBundleMode)
            {
                string bundleName = string.Empty;
                AssetBundleManager bundleMgr = AssetBundleManager.Instance;
                if (!bundleMgr.assetbundleMap.TryGetValue(assetPath.ToLower(), out bundleName))
                {
                    bundleName = string.Format("scenes/{0}.ab", assetPath);
                }
                yield return bundleMgr.LoadSceneDependency(bundleName);

                string scenePath = string.Format("{0}{1}", Util.DataPath, bundleName);
                if (File.Exists(scenePath))
                {
                    abcr = AssetBundle.LoadFromFileAsync(scenePath);
                    yield return abcr;
                }
            }

            OnLoad();

            yield return null;
        }

        public override bool MoveNext()
        {
            if (!hasLoaded)
            {
                hasLoaded = true;
                AppInter.Instance.StartCoroutine(loadAsync());
            }

            if (loading != null)
            {
                progress = loading.progress;
                if (progress >= 0.9f)
                {
                    loading.allowSceneActivation = true;
                }
            }

            bool result = IsDone();
            if (result) this.onFinishEvent();

            return !result;
        }

        public override bool IsDone()
        {
            if (loading != null && loading.isDone)
                return true;
            return false;
        }

        public override void Reset()
        {
            base.Reset();

            if (AppConst.AssetBundleMode)
            {
                AssetBundleManager.Instance.UnloadSceneDependency();

                if (abcr != null)
                {
                    abcr.assetBundle.Unload(false);
                    abcr = null;
                }
            }
        }

        [NoToLua]
        public override T GetAsset<T>()
        {
            return default(T);
        }

    }
}