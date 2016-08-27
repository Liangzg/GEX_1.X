/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace GOE.Scene
{
    /// <summary>
    /// 描述：场景管理器，用来管理场景的加载方式，缓存数据
    /// <para>创建时间：2016-06-15</para>
    /// </summary>
    public sealed class SceneManager {
        
        public ASceneLoader CurLoader;
        private static SceneManager mInstance;

        private ARCCache<string, AssetBundle> cache = new ARCCache<string, AssetBundle>(5);

        public class Transitions
        {
            public const string FadeTransition = "Transitions/SMFadeTransition";
            public const string BlindsTransition = "Transitions/SMBlindsTransition";
        }

        private SceneManager()
        {
            CurLoader = new SceneNomalLoader();    
        }

        public static SceneManager Instance
        {
            get
            {
                if(mInstance == null)
                    mInstance = new SceneManager();
                return mInstance;
            }
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="transition">过滤资源相对路径</param>
        /// <param name="callback">异步加载完成后的回调</param>
        /// <returns>场景加载器，可用于跟踪进度</returns>
        public void LoadSceneAsync(string sceneName, string transition , Action callback, Action<float, float> progress)
        {
            loadScene(sceneName, transition, callback, true , progress);
        }


        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="transition">过滤资源相对路径</param>
        public void LoadScene(string sceneName, string transition)
        {
            loadScene(sceneName, transition, null, false, null);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="transition">过滤资源相对路径</param>
        /// <param name="callback">异步加载完成后的回调</param>
        /// <param name="isLoadAsync">是否异步加载</param>
        /// <returns>场景加载器，可用于跟踪进度</returns>
        private static void loadScene(string sceneName, string transition, Action callback, bool isLoadAsync, Action<float, float> progress)
        {
            Instance.CurLoader.IsLoadAsync = isLoadAsync;
            Instance.CurLoader.AddFinish(callback);
            Instance.CurLoader.SetProgress(progress);

            UnityEngine.Object obj = Resources.Load(transition);
            if (obj == null)
            {
                throw new ArgumentException("Cant find Object ! Transition Path is " + transition);
            }

            GameObject gObj = (GameObject)GameObject.Instantiate(obj);
            ASceneTransition trans = gObj.GetComponent<ASceneTransition>();
            trans.screenName = sceneName;
            trans.Loader = Instance.CurLoader;
        }

    }
}

