using UnityEngine;
using System.Collections.Generic;

namespace GEX.Resource
{
    public class TextAssetWatcher : MonoBehaviour {

        public List<string> bundleNames = new List<string>();

        public void AddBundleName(string bundleName)
        {
            bundleNames.Add(bundleName);
        }

        void OnDestroy()
        {
            if (AssetBundleManager.Instance != null)
            {
                for (int i = 0; i < bundleNames.Count; ++i)
                {
                    AssetBundleManager.Instance.UnloadAsset(bundleNames[i]);
                }
            }
        }
    }

}

