using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace GEX.Resource
{
    /// <summary>
    /// 串形异步加载容器,容器内部自动缓存加载器,需要手动释放
    /// 使用方法：
    ///     var async = new SerialAsync();
    ///     
    ///     async.AddLoader("xxx/press/yy.prefab");
    ///     async.AddLoader("xxx/pres/zzz.ogg" , 5);
    /// 
    ///     while(async.MoveNext())
    ///     {
    ///         if(async.CurrentLoader.IsDone()){
    ///             var loader = async.CurrentLoader;
    ///             var gameObj = GameObject.Instantiate(loader.GetAsset<GameObject>());
    ///         }
    ///         // progress
    ///         if(progressAction != null)
    ///              progressAction.Invoke(async.Progress);
    ///         yield return null;
    ///     }
    /// </summary>
    public class SerialLoader : ALoadOperation
    {
        
        private class AsyncLoader
        {
            public int Weight;

            public ALoadOperation Loader;
        }
        /// <summary>
        /// 权重，占总场景资源量的比重
        /// </summary>
        public int Weight { get; private set; }
        
        private int completeWeight;

        private List<AsyncLoader> assets = new List<AsyncLoader>();

        private AsyncLoader curLoader;
        private AsyncLoader nextLoader;
        private int moveIndex;

        //缓存
        private Dictionary<ALoadOperation, AsyncLoader> cacheLoader;

        public SerialLoader()
        {
            cacheLoader = new Dictionary<ALoadOperation, AsyncLoader>();
        }
        
        /// <summary>
        /// 添加需要被加载的资源
        /// </summary>
        /// <param name="loader">异步加载器</param>
        /// <param name="weight">权重，用于计算进度</param>
        public void AddLoader(ALoadOperation loader, int weight)
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
        /// 添加加载数据
        /// </summary>
        /// <param name="path">资源路径,类似"Assets/Res/XXXX.yyy"</param>
        /// <param name="weight">权重，用于计算进度</param>
        public void AddLoader(string path, int weight = 1)
        {
            this.AddLoader(GResource.LoadBundleAsync(path) , weight);            
        }

        public override bool MoveNext()
        {
            if (assets.Count <= 0) return false;

            bool isMoveNext = false;

            AsyncLoader temLoader = null;
            if (cacheLoader.TryGetValue(nextLoader.Loader, out temLoader))
            {
                nextLoader.Loader.Finish(temLoader.Loader);
                isMoveNext = true;
            }else
                isMoveNext = !nextLoader.Loader.MoveNext();

            if (isMoveNext)
            {
                moveIndex++;

                curLoader = nextLoader;
                
                if (!cacheLoader.TryGetValue(curLoader.Loader, out temLoader))
                {
                    cacheLoader[curLoader.Loader] = curLoader;
                }
                
                if (moveIndex < assets.Count)
                {
                    completeWeight += nextLoader.Weight;
                    nextLoader = assets[moveIndex];
                }
            }

            float completedProgress = completeWeight + nextLoader.Loader.Progress * nextLoader.Weight;
            progress = completedProgress / Weight;

            return !IsDone();
        }

        public ALoadOperation CurrentLoader
        {
            get { return curLoader.Loader; }
        }

        public override object Current
        {
            get { return CurrentLoader; }
        }
        
        public override bool IsDone()
        {
            bool result = moveIndex >= assets.Count;
            if(result)
                this.onFinishEvent();
            return result;
        }

        public override void Reset()
        {
            moveIndex = 0;
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].Loader.Reset();
                assets[i].Loader.OnFinish = null;
            }
            assets.Clear();

            cacheLoader.Clear();

            nextLoader = null;
            curLoader = null;

            progress = 0;
            Weight = 0;
        }

        public override void OnLoad()
        {
            throw new System.NotImplementedException();
        }

        [NoToLua]
        public override T GetAsset<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}
