using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

namespace LuaDataBind
{
    public class LuaContext : MonoBehaviour
    {
         
        private List<IBinding> binders = new List<IBinding>();
        /// <summary>
        /// Lua 数据Model
        /// </summary>
        public LuaTable LuaModel { get; set; }
        /// <summary>
        /// 视图逻辑Lua
        /// </summary>
        public LuaTable LuaView { get; set; }
        public IBinding[] Binders
        {
            get
            {
                if (binders.Count == 0)
                {
                    Setter[] bindingSetterArr = this.GetComponentsInChildren<Setter>();
                    binders.AddRange(bindingSetterArr);
                }
                return binders.ToArray();
            }
        }

        private void Awake()
        {

        }


        
        private void OnDestroy()
        {

        }

        private void OnDisable()
        {

        }

        public void RegistBinder(IBinding binding)
        {
            if (binders.Contains(binding)) return;

            binders.Add(binding);
            
            object defalutValue = GetValue(binding);
            if(defalutValue != null)
                binding.OnObjectChanged(defalutValue);
        }


        public void UnregistBinder(IBinding binding)
        {
            if (!binders.Contains(binding)) return;
            binders.Remove(binding);
        }

        public object GetValue(IBinding binding)
        {
            if (binding.BindingType == DataBindingType.Constant)
                return binding.Path;
            string key = binding.Path;
            if (LuaModel == null || LuaModel[key] == null) return null;

            return LuaModel[key];
        }

        public void SetValue(string key, object newValue)
        {
            if (LuaModel == null ) return ;
            LuaModel[key] = newValue;
        }
    }
}