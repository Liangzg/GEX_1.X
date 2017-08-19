using System;

namespace LuaDataBind
{
    public abstract class SingleSetter<T> : Setter
    {
        
        public override void OnObjectChanged(object value)
        {
            object val = getValue(value);
            T newVal = val is T ? (T)val : default(T);


            this.OnValueChanged(newVal);
        }

        protected abstract void OnValueChanged(T newValue);


        private object getValue(object value)
        {
            object val = Convert.ChangeType(value, typeof(T));
            switch (Type)
            {
                    case DataBindingType.Provider:
                    if (Provider != null)
                    {
                        Provider.OnObjectChanged(val);
                        return Provider.Value;
                    }
                    break;
                    case DataBindingType.Constant:

                    break;
            }
            return value;
        }
    }
}