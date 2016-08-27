/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GOE.Scene
{
    /// <summary>
    /// 描述：场景加载器基类，提供统一的加载入口
    /// <para>创建时间：2016-06-15</para>
    /// </summary>
    public abstract class ASceneLoader
    {
        /// <summary>
        /// 是否进行异步加载
        /// </summary>
        public bool IsLoadAsync = true;

        private List<Action> finishCallback;
        /// <summary>
        /// 加载进度, （每次的变化值，总的变化值）
        /// </summary>
        protected Action<float, float> progress;
        /// <summary>
        /// 加载场景资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="delay">延迟时间</param>
        /// <returns></returns>
        public IEnumerator OnLoad(string sceneName)
        {
            IEnumerator loadLevel = onLoadLevel(sceneName);
            while (loadLevel.MoveNext())
                yield return 0;

            OnFinish();
        }

        /// <summary>
        /// 具体的加载逻辑
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator onLoadLevel(string sceneName);
        
        /// <summary>
        /// 添加完成后的回调
        /// </summary>
        /// <param name="callback"></param>
        public void AddFinish(Action callback)
        {
            if (callback == null) return;

            if (finishCallback == null)
                finishCallback = new List<Action>();
            
            //防止重复添加
            if (finishCallback.Contains(callback)) return;
            
            finishCallback.Add(callback);
        }

        /// <summary>
        /// 删除完成时的回调
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveFinish(Action callback)
        {
            if (finishCallback == null) return;

            finishCallback.Remove(callback);
        }

        /// <summary>
        /// 清理全部完成时的回调
        /// </summary>
        public void ClearFinish()
        {
            if(finishCallback != null)
                finishCallback.Clear();
            finishCallback = null;
        }


        public void OnFinish()
        {
            if (finishCallback != null)
            {
                foreach (Action action in finishCallback)
                {
                    action.Invoke();
                }
                finishCallback.Clear();
            }
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected IEnumerator load(string sceneName , UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (IsLoadAsync)
            {
                WWW _www = new WWW("");
                //进度数据传递
                int totalProgress = 0 , curTotalProgress;
                int offset = 0;
                while (!_www.isDone)
                {
                    curTotalProgress = (int)(_www.progress * 100);
                    offset = curTotalProgress - totalProgress;
                    totalProgress = curTotalProgress;
                    InvokeProgress(offset, totalProgress);
                    yield return null;
                }

                curTotalProgress = totalProgress == 0 ? (int)(_www.progress * 100) : totalProgress;
                offset = 100 - curTotalProgress;
                totalProgress = 100;
                InvokeProgress(offset, totalProgress);
                while (curTotalProgress < totalProgress)
                {
                    curTotalProgress++;
                    yield return null;
                }
                //清空进度委托
                this.SetProgress(null);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName , mode);
                yield return 0;
            }
        }

        /// <summary>
        /// 设置进度监听
        /// </summary>
        /// <param name="callback"></param>
        public void SetProgress(Action<float, float> callback)
        {
            progress = callback;
        }

        /// <summary>
        /// 执行进度监听
        /// </summary>
        /// <param name="progressOffset"></param>
        /// <param name="totalPress"></param>
        public void InvokeProgress(float progressOffset, float totalPress)
        {
            if (progress == null) return;

            progress.Invoke(progressOffset , totalPress);
        }
    }
}

