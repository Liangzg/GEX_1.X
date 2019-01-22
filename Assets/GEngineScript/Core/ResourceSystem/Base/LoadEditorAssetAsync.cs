using System;
using LuaInterface;

namespace GEX.Resource
{
    /// <summary>
    /// 加载开发期的资源
    /// </summary>
    public class LoadEditorAssetAsync : ALoadOperation
    {

        private UnityEngine.Object mainAsset;
        

        private enum ELoadType
        {
            AssetPath , Prefab 
        }

        private ELoadType loadType;
        
        public LoadEditorAssetAsync(string path)
            : base(path)
        {
            loadType = ELoadType.AssetPath;
        }

        public LoadEditorAssetAsync(UnityEngine.Object asset , string path)
            : base(path)
        {
            mainAsset = asset;
            loadType = ELoadType.Prefab;
        }

        public override void OnLoad()
        {
#if UNITY_EDITOR
            if(loadType == ELoadType.AssetPath)
            {
                mainAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (mainAsset == null)
                    UnityEngine.Debug.LogError("Cant find Asset! " + assetPath);
            }
#endif
        }

        public override bool IsDone()
        {
            this.onFinishEvent();

            return true;
        }

        [NoToLua]
        public override T GetAsset<T>()
        {
            return (T)mainAsset;
        }
    }
}
