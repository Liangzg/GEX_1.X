/************************************************************************************
 *  @Author : LiangZG
 *  @Email ： game.liangzg@foxmail.com
 *  @Date : 2017-04-27
 ***********************************************************************************/

using System.Collections;
using LuaInterface;
using System;
using GEX.Resource;
using LuaFramework;
using UnityEngine;

namespace GEX.SceneManager
{
    /// <summary>
    /// 游戏场景管理
    /// </summary>
    public sealed class SceneStageManager : ASingleton<SceneStageManager>
    {
        public LuaTable curStage { get; private set; }
        /// <summary>
        /// 异步场景加载器
        /// </summary>
        public StageLoader stageLoader { get; private set; }
        
        private LuaManager luaMgr;
        private string nextSceneName;
        /// <summary>
        /// 下一个场景的名称
        /// </summary>
        public string NextSceneName
        {
            get { return nextSceneName; }
        }

        void Awake()
        {
            luaMgr = AppFacade.Instance.GetManager<LuaManager>();
        }

        public void LoadScene(LuaTable newStage)
        {
            string sceneName = newStage.GetStringField("stageName");
            luaMgr.StartCoroutine(loadScene(sceneName , newStage));
        }

        private IEnumerator loadScene(string sceneName , LuaTable newStage)
        {
            LuaTable lastStage = curStage;
            curStage = newStage;

            
            CallFunction(lastStage, "onExit");
            
            stageLoader = new StageLoader();
            CallFunction(newStage, "onEnter" , stageLoader);
            
            while (stageLoader.MoveNext())
                yield return null;

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

            while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != sceneName)
                yield return null;

            //wait a frame 
            yield return null;
     
            this.OnCompleted();

            CallFunction(newStage, "onShow");
        }

        /// <summary>
        /// 异步加载本地当前场景切换
        /// </summary>
        /// <param name="newStage">场景Lua状态</param>
        /// <param name="process">加载进度</param>
        public void LoadLocalScene(LuaTable newStage , Action<float> process)
        {
            LuaTable lastStage = curStage;
            curStage = newStage;

            stageLoader = new StageLoader();
            
            CallFunction(lastStage, "onExit");

            luaMgr.StartCoroutine(this.onLoading(process));
        }

        private IEnumerator onLoading(Action<float> process)
        {
            CallFunction(curStage, "onEnter", this, stageLoader);

            float progress = 0f;
            while (stageLoader.MoveNext())
            {
                progress = Mathf.Lerp(progress, stageLoader.Progress, Time.deltaTime * 10f);

                if (process != null)
                    process.Invoke(progress);

                yield return null;
            }

            if (process != null) process.Invoke(1);

            while (!stageLoader.IsSceneDone())
                yield return null;
            
            OnCompleted();

            CallFunction(curStage, "onShow");
        }
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="newStage">场景Lua状态</param>
        public void LoadSceneViaPreloading(LuaTable newStage)
        {
            LuaTable lastStage = curStage;
            curStage = newStage;

            //下个场景名
            string sceneName = newStage.GetStringField("stageName");
            nextSceneName = sceneName;

            //过渡场景名
            string transitScene = newStage.GetStringField("transitScene");
            stageLoader = new StageLoader(sceneName);

            CallFunction(lastStage, "onExit");

            UnityEngine.SceneManagement.SceneManager.LoadScene(transitScene);
        }

        /// <summary>
        /// 加载切块的场景
        /// </summary>
        /// <param name="newStage">场景Lua状态</param>
        public void LoadChunkScene(LuaTable newStage)
        {
            LuaTable lastStage = curStage;
            curStage = newStage;

            //下个场景名
            string sceneName = newStage.GetStringField("stageName");
            nextSceneName = sceneName;

            //过渡场景名
            string transitScene = newStage.GetStringField("transitScene");
            stageLoader = new StageLoader();

            CallFunction(lastStage, "onExit");

            UnityEngine.SceneManagement.SceneManager.LoadScene(transitScene);
        }

        public static void CallFunction(LuaTable luaTab, string func , params object[] args)
        {
            if (luaTab == null) return;

            LuaFunction luaFunc = luaTab.GetLuaFunction(func);
            luaFunc.BeginPCall();
            luaFunc.Push(luaTab);
            luaFunc.PushArgs(args);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }

        public void OnCompleted()
        {
            if(stageLoader != null)
                stageLoader.Reset();
            stageLoader = null;
        }
    }
}
