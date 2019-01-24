/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using System.Collections;
using GEX.Resource;
using GEX.SceneManager;
using UnityEngine;

namespace GOE.Scene
{

    /// <summary>
    /// 描述：场景过滤基类
    /// <para>创建时间：2016-06-15</para>
    /// </summary>
    public abstract class ASceneTransition : MonoBehaviour
    {

        protected SMTransitionState state = SMTransitionState.Out;

        public StageLoader Loader;

        /// <summary>
        /// Gets the current state of the transition. This is read-only, as the state is controlled by the transition 
        /// framework.
        /// </summary>
        /// <remarks>@since version 1.4.5</remarks>
        public SMTransitionState CurrentTransitionState
        {
            get
            {
                return state;
            }
        }

        public bool timeScaleIndependent = true;

        // Prefetching needs Unity 4 therefore we hide it in Unity 3.5
        public bool prefetchLevel = false;
        /// <summary>
        /// The id of the screen that is being loaded.
        /// </summary>
        [HideInInspector]
        public string screenName;

        protected SceneStageManager sceneStageMgr;

        void Start()
        {
            sceneStageMgr = SceneStageManager.Instance;
            Loader = sceneStageMgr.stageLoader;

            if (Time.timeScale <= 0f && !timeScaleIndependent)
            {
                Debug.LogWarning("Time.timeScale is set to 0 and you have not enabled 'Time Scale Independent' at the transition prefab. " +
                                 "Therefore the transition animation would never start to play. Please either check the 'Time Scale Independent' checkbox" +
                                 "at the transition prefab or set Time.timeScale to a positive value before changing the level.", this);
                return; // do not do anything in this case.
            }


            if (prefetchLevel)
            {
                Debug.LogWarning("You can only prefetch the level when using asynchronous loading. " +
                    "Please either uncheck the 'Prefetch Level' checkbox on your level transition prefab or check the " +
                    "'Load Async' checkbox. Note, that asynchronous loading (and therefore level prefetching) requires a Unity Pro license.", this);
                return; // don't do anything in this case.
            }

            StartCoroutine(DoTransition());
        }

        /// <summary>
        /// This method actually does the transition. It is run in a coroutine and therefore needs to do
        /// yield returns to play an animation or do another progress over time. When this method returns
        /// the transition is expected to be finished.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator"/> for showing the transition status. Use yield return statements to keep
        /// the transition running, otherwise simply end the method to stop the transition.
        /// </returns>
        protected virtual IEnumerator DoTransition()
        {
            // make sure the transition doesn't get lost when switching the level.
            DontDestroyOnLoad(gameObject);

            if (prefetchLevel)
            {
                state = SMTransitionState.Prefetch;
                SendMessage("SMBeforeTransitionPrefetch", this, SendMessageOptions.DontRequireReceiver);
                yield return 0; // wait one frame
                SendMessage("SMOnTransitionPrefetch", this, SendMessageOptions.DontRequireReceiver);

                if (Loader != null)
                {
                    while (Loader.MoveNext())
                    {
                        yield return 0;
                    }
                }
            }

            state = SMTransitionState.Out;
            SendMessage("SMBeforeTransitionOut", this, SendMessageOptions.DontRequireReceiver);
            Prepare();
            SendMessage("SMOnTransitionOut", this, SendMessageOptions.DontRequireReceiver);
            float time = 0;

            while (Process(time))
            {
                time += DeltaTime;
                // wait for the next frame
                yield return 0;
            }

            SendMessage("SMAfterTransitionOut", this, SendMessageOptions.DontRequireReceiver);
            // wait another frame...
            yield return 0;

            state = SMTransitionState.Hold;
            SendMessage("SMBeforeTransitionHold", this, SendMessageOptions.DontRequireReceiver);
            SendMessage("SMOnTransitionHold", this, SendMessageOptions.DontRequireReceiver);

            // wait another frame...
            yield return 0;


            if (!prefetchLevel && Loader != null)
            {
                // level is not prefetched, load it right now.
                while (Loader.MoveNext())
                {
                    yield return 0;
                }
            }

            SendMessage("SMAfterTransitionHold", this, SendMessageOptions.DontRequireReceiver);
            // wait another frame...
            yield return 0;

            state = SMTransitionState.In;
            SendMessage("SMBeforeTransitionIn", this, SendMessageOptions.DontRequireReceiver);
            Prepare();
            SendMessage("SMOnTransitionIn", this, SendMessageOptions.DontRequireReceiver);
            time = 0;

            while (Process(time))
            {
                time += DeltaTime;
                // wait for the next frame
                yield return 0;
            }

            sceneStageMgr.OnCompleted();
            SendMessage("SMAfterTransitionIn", this, SendMessageOptions.DontRequireReceiver);
            // wait another frame...
            yield return 0;

            Destroy(gameObject);
        }

        /// <summary>
        /// invoked at the start of the <see cref="SMTransitionState.In"/> and <see cref="SMTransitionState.Out"/> state to 
        /// initialize the transition
        /// </summary>
        protected virtual void Prepare()
        {
        }

        /// <summary>
        /// Invoked once per frame while the transition is in state <see cref="SMTransitionState.In"/> or <see cref="SMTransitionState.Out"/> 
        /// to calculate the progress
        /// </summary>
        /// <param name='elapsedTime'>
        /// the time that has elapsed since the start of current transition state in seconds. 
        /// </param>
        /// <returns>
        /// false if no further calls are necessary for the current state, true otherwise
        /// </returns>
        protected abstract bool Process(float elapsedTime);

        /// <summary>
        /// Gets the delta time according to the settings. If realTimeScaling is enabled, the time scale will not affect
        /// the speed at which the transition is playing. Otherwise a change to the time scale will affect the speed
        /// at which transitions are played.
        /// </summary>
        /// <value>The delta time.</value>
        protected virtual float DeltaTime
        {
            get
            {
                if (timeScaleIndependent)
                {
                    return SMRealTimeHelper.deltaTime;
                }
                else {
                    return Time.deltaTime;
                }
            }
        }
    }
}

