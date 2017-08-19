using UnityEngine;

namespace LuaDataBind
{
    public abstract class DataProvider : MonoBehaviour
    {
        /// <summary>
        ///   Current data value.
        /// </summary>
        public abstract object Value { get; }

        public abstract void OnObjectChanged(object value);
    }
}