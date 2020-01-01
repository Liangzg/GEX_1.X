using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace GEX.Resource
{
    /// <summary>
    /// 并行异步加载容器,容器内部自动缓存加载器,需要手动释放
    /// 使用方法：
    ///     var async = new ParallelAsync();
    ///     
    ///     async.AddLoader("xxx/press/yy.prefab");
    ///     async.AddLoader("xxx/pres/zzz.ogg" , 5);
    /// 
    ///     while(async.MoveNext())
    ///     {
    ///         // progress
    ///         if(progressAction != null)
    ///              progressAction.Invoke(async.Progress);
    ///         yield return null;
    ///     }
    /// </summary>
    public class ParallelLoader : LoadOperation
    {
        
        private class AsyncLoader
        {
            public int Weight;

            public LoadOperation Loader;
        }
        /// <summary>
        /// 权重，占总场景资源量的比重
        /// </summary>
        public int Weight { get; private set; }
        
        private float completeWeight;
        private int maxParallelCount = 5;
        
        private List<AsyncLoader> loaders = new List<AsyncLoader>(); 

        private List<int> finishs = new List<int>(); 

        public ParallelLoader() : base("")
        {
        }

        public ParallelLoader(int maxParallelCount)
            : this()
        {
            this.maxParallelCount = maxParallelCount;
        }
        
        /// <summary>
        /// 添加需要被加载的资源
        /// </summary>
        /// <param name="loader">异步加载器</param>
        /// <param name="weight">权重，用于计算进度</param>
        public LoadOperation AddLoader(LoadOperation loader, int weight = 1)
        {
            Weight += weight;

            AsyncLoader asyncLoader = new AsyncLoader();
            asyncLoader.Weight = weight;
            asyncLoader.Loader = loader;

            this.loaders.Add(asyncLoader);

            return loader;
        }

        /// <summary>
        /// 添加加载数据
        /// </summary>
        /// <param name="path">资源路径,类似"Assets/Res/XXXX.yyy"</param>
        /// <param name="weight">权重，用于计算进度</param>
        public LoadOperation AddLoader(string path, int weight = 1)
        {
            LoadOperation loadOpt = GResource.LoadAssetAsync(path);
            this.AddLoader(loadOpt, weight);
            return loadOpt;
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

        public override bool MoveNext()
        {
            if (loaders.Count <= 0) return false;

            float weigeting = 0;
            for (int i = 0 , count = loaders.Count; i < count; i++)
            {
                if (i >= maxParallelCount) break;

                AsyncLoader asynloader = loaders[i];
                if (!asynloader.Loader.MoveNext())
                {
                    completeWeight += asynloader.Weight;
                    finishs.Add(i);
                }
                else
                {
                    weigeting += asynloader.Loader.Progress * asynloader.Weight;
                }
            }

            if (finishs.Count > 0)
            {
                finishs.Sort((x , y)=>x.CompareTo(y) * -1);
                for (int i = 0; i < finishs.Count; i++)
                {
                    loaders.RemoveAt(finishs[i]);
                }
                finishs.Clear();
            }
            
            progress = (weigeting + completeWeight) / Weight;

            return IsDone() == false;
        }

        public override object Current
        {
            get { return null; }
        }
        
        public override bool IsDone()
        {
            bool result = loaders.Count <= 0;
            if(result)  this.onFinishEvent();
            return result;
        }

        public override void Reset()
        {
            for (int i = 0; i < loaders.Count; i++)
            {
                loaders[i].Loader.Reset();
            }
            loaders.Clear();
            Weight = 0;
        }
    }
}
