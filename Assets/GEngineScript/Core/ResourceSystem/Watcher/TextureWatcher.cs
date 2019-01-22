using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GEX.Resource
{
   public class TextureWatcher : MonoBehaviour {

    public List<string> bundleNames = new List<string>();

    private bool loaded = false;

    public void AddBundleName(string bundleName)
    {
        bundleNames.Add(bundleName);
        // 假如当前Go未激活,这里需要卸载一次,防止某些情况下Go一直是disable状态,在删除时不会执行OnDisable
        if (!gameObject.activeSelf)
            AssetBundleManager.Instance.UnloadAsset(bundleName);
    }

    private void OnEnable()
    {
        if (!loaded)
        {
            // 激活时增加一次refCount
            for (int i = 0; i < bundleNames.Count; ++i)
            {
                AssetBundleManager.Instance.LoadAsset(bundleNames[i]);
            }
        }
        loaded = true;
    }

    private void OnDisable()
    {
        loaded = false;
        // 关闭激活时减小refCount
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

