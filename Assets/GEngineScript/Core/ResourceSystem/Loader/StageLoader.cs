using System.Collections;
using System.Collections.Generic;

namespace GEX.Resource
{
    /// <summary>
    /// 异步加载场景
    /// </summary>
    public class StageLoader : IEnumerator
    {
        public class AsyncLoader
        {
            public int Weight;

            public LoadOperation Loader;
        }
        /// <summary>
        /// 权重，占总场景资源量的比重
        /// </summary>
        public int Weight { get; private set; }

        /// <summary>
        /// 当前加载总进度
        /// </summary>
        public float Progress { get; private set; }

        private int completeWeight;

        private List<AsyncLoader> assets = new List<AsyncLoader>();

        private AsyncLoader curLoader;
        private AsyncLoader nextLoader;
        private int moveIndex;

        private LoadSceneAsync sceneLoader;

        public LoadSceneAsync SceneLoader
        {
            get { return sceneLoader; }
        }

        public StageLoader()
        {
        }

        public StageLoader(string sceneName)
        {
            sceneLoader = new LoadSceneAsync(sceneName);
            this.AddLoader(sceneLoader , UnityEngine.Random.Range(70, 90));
        }

        /// <summary>
        /// 添加场景资源加载器
        /// </summary>
        /// <param name="loader">加载器</param>
        /// <param name="weight">资源权重</param>
        public void AddLoader(LoadOperation loader , int weight)
        {
            Weight += weight;

            AsyncLoader asyncLoader = new AsyncLoader();
            asyncLoader.Weight = weight;
            asyncLoader.Loader = loader;

            this.assets.Add(asyncLoader);

            if (nextLoader == null)
            {
                nextLoader = asyncLoader;
                curLoader = nextLoader;
            }
        }

        /// <summary>
        /// 批量添加资源加载器
        /// </summary>
        /// <param name="loaders">加载器集合</param>
        /// <param name="weight">总资源权重</param>
        public void AddRangeLoader(LoadOperation[] loaders , int weight)
        {
            int childWeight = weight/loaders.Length;
            Weight += childWeight * loaders.Length;

            for (int i = 0; i < loaders.Length; i++)
            {
                AsyncLoader asyncLoader = new AsyncLoader();
                asyncLoader.Weight = childWeight;
                asyncLoader.Loader = loaders[i];

                this.assets.Add(asyncLoader);
            }

            if (nextLoader == null)
            {
                nextLoader = this.assets[0];
                curLoader = nextLoader;
            }
        }
        /// <summary>
        /// 自定义排序规则
        /// </summary>
        public void Sort(IComparer<AsyncLoader> comparer)
        {
            this.assets.Sort(comparer);
        }

        public bool MoveNext()
        {
            if (assets.Count <= 0) return false;

            if (!nextLoader.Loader.MoveNext())
            {
                moveIndex++;

                curLoader = nextLoader;

                if (moveIndex < assets.Count)
                {
                    completeWeight += nextLoader.Weight;
                    nextLoader = assets[moveIndex];
                }
            }

            float completedProgress = completeWeight + nextLoader.Loader.Progress * nextLoader.Weight;
            Progress = completedProgress / Weight;

            return !IsDone();
        }
        /// <summary>
        /// 立即激活场景切换
        /// </summary>
        public void OnActiveImmediate()
        {
            if (sceneLoader == null) return;

            sceneLoader.AsyncSceneLoader.allowSceneActivation = true;
        }

        /// <summary>
        /// 异步场景是否加载完毕
        /// </summary>
        /// <returns></returns>
        public bool IsSceneDone()
        {
            if (sceneLoader == null) return true;

            return sceneLoader.AsyncSceneLoader.isDone;
        }

        public object Current
        {
            get { return curLoader.Loader; }
        }

        public bool IsDone()
        {
            return moveIndex > assets.Count;
        }

        public void Reset()
        {
            if(sceneLoader != null)
                sceneLoader.Reset();
            sceneLoader = null;

            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].Loader.Reset();
            }
            assets.Clear();

            curLoader = null;
            nextLoader = null;
        }
    }
}