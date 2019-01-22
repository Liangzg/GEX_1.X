using UnityEngine;

namespace GEX.Resource
{
    public sealed class AssetWatcher : MonoBehaviour
    {
        public string bundleName;

        private bool loaded = false;

        void OnEnable()
        {
            if (!loaded)
                AssetBundleManager.Instance.LoadAsset(bundleName, gameObject);
            loaded = true;
        }

        void OnDisable()
        {
            loaded = false;
            if (AssetBundleManager.Instance != null)
                AssetBundleManager.Instance.UnloadAsset(bundleName, gameObject);
        }
    }
}
