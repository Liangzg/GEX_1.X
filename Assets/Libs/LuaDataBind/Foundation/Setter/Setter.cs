using UnityEngine;

namespace LuaDataBind
{
    public abstract class Setter : MonoBehaviour , IBinding
    {
        public string path;

        /// <summary>
        ///   Type of data binding.
        /// </summary>
        public DataBindingType Type;

        [HideInInspector]
        public DataProvider Provider;

        public string Path { get { return path; } }
        
        public DataBindingType BindingType {
            get { return Type; }
        }
        public LuaContext Context { get; set; }

        protected virtual void Awake()
        {
            if (Provider == null)
                Provider = this.GetComponent<DataProvider>();
        }

        protected virtual void Start()
        {
            LuaContext parentContext = this.GetComponentInParent<LuaContext>();
            parentContext.RegistBinder(this);
            Context = parentContext;
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {
            Context.UnregistBinder(this);
        }


        public virtual void OnObjectChanged(object value)
        {
            
        }
    }
}