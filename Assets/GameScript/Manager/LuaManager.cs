using UnityEngine;
using System.Collections;
using LuaInterface;

namespace LuaFramework {
    public class LuaManager : BaseLuaManager {

        private LuaLoader loader;

        // Use this for initialization
        protected void Awake() {
            loader = new LuaLoader();
            base.Awake();
        }

        public void InitStart() {
            InitLuaBundle();
            base.InitStart();
            this.StartMain();
        }


        void StartMain() {
            lua.DoFile("AppMain.lua");

            LuaFunction main = lua.GetFunction("AppMain.Main");
            main.Call();
            main.Dispose();
        }
        
        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        void InitLuaBundle() {
            if (loader.beZip) {
                loader.AddBundle("lua/lua.unity3d");
                loader.AddBundle("lua/lua_math.unity3d");
                loader.AddBundle("lua/lua_system.unity3d");
                loader.AddBundle("lua/lua_system_reflection.unity3d");
                loader.AddBundle("lua/lua_unityengine.unity3d");
                loader.AddBundle("lua/lua_common.unity3d");
                loader.AddBundle("lua/lua_logic.unity3d");
                loader.AddBundle("lua/lua_view.unity3d");
                loader.AddBundle("lua/lua_controller.unity3d");
                loader.AddBundle("lua/lua_misc.unity3d");

                loader.AddBundle("lua/lua_protobuf.unity3d");
                loader.AddBundle("lua/lua_3rd_cjson.unity3d");
                loader.AddBundle("lua/lua_3rd_luabitop.unity3d");
                loader.AddBundle("lua/lua_3rd_pbc.unity3d");
                loader.AddBundle("lua/lua_3rd_pblua.unity3d");
                loader.AddBundle("lua/lua_3rd_sproto.unity3d");
            }
        }


 

        public void Close() {
            LuaFunction destroy = lua.GetFunction("AppMain.Destroy");
            destroy.Call();
            destroy.Dispose();
            
            loader = null;

            base.Close();
        }
    }
}