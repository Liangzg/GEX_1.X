using LuaInterface;
using UnityEngine;

namespace LuaDataBind
{
    public abstract class Command : MonoBehaviour
    {

        public string Func;

        [HideInInspector]
        public LuaContext Context;

        protected virtual void Awake()
        {
            if(Context == null)
                Context = this.GetComponentInParent<LuaContext>();
        }

        protected virtual  void OnDestroy()
        {

        }

        protected abstract void invokeCommand();
    }
}