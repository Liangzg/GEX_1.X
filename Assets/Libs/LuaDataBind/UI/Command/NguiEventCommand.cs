using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace LuaDataBind
{
    /// <summary>
    ///   Base class for a command when a NGUI event on a widget occured.
    /// </summary>
    /// <typeparam name="TWidget">Type of widget to monitor.</typeparam>
    public abstract class NguiEventCommand<TWidget> : Command
        where TWidget : MonoBehaviour
    {

        /// <summary>
        ///   Target widget to work with.
        /// </summary>
        [HideInInspector]
        public TWidget Target;


        protected override void Awake()
        {
            base.Awake();

            if (this.Target == null)
            {
                this.Target = this.GetComponent<TWidget>();
            }
        }

        /// <summary>
        ///   Returns the event from the specified target to observe.
        /// </summary>
        /// <param name="target">Target behaviour to get event from.</param>
        /// <returns>Event from the specified target to observe.</returns>
        protected abstract List<EventDelegate> GetEvent(TWidget target);

        /// <summary>
        ///   Unity callback.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (this.Target != null)
            {
                EventDelegate.Remove(this.GetEvent(this.Target), this.OnEvent);
            }
        }

        /// <summary>
        ///   Unity callback.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (this.Target != null)
            {
                EventDelegate.Add(this.GetEvent(this.Target), this.OnEvent);
            }
        }

        /// <summary>
        ///   Called when the observed event occured.
        /// </summary>
        protected virtual void OnEvent()
        {
            this.invokeCommand();
        }

        protected override void invokeCommand()
        {
            if (Context == null || Context.LuaView == null) return;

            LuaTable modelTable = Context.LuaView;

            LuaFunction func = modelTable.GetLuaFunction(Func);
            if (func == null)
            {
                Debug.LogWarning("Cant find Lua Function ! Function name is " + Func);
                return;
            }

            func.BeginPCall();
            func.Push(modelTable);
            func.Push(this.Target);
            func.PCall();
            func.EndPCall();
        }
    }
}