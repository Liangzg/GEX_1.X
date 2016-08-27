/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 描述：应用中转脚本，一直存在于场景
/// 用处说明：
///     1.用于启动一个不会销毁的协同
///     2.用于跨平台业务中转，比如SDK接入后的数据交互
/// <para>创建时间：</para>
/// </summary>
public class AppInter 
{
    private static AppInterMono mInstance;
    
    public static AppInterMono Instance
    {
        get
        {
            if(mInstance != null)   return mInstance;
            
            GameObject gObj = new GameObject("_App");
            mInstance = gObj.AddComponent<AppInterMono>();

            return mInstance;
        }
    }
    /// <summary>
    /// 启动协程，该协程不会随场景的切换而停止
    /// </summary>
    /// <param name="method"></param>
    public static Coroutine StartCoroutine(IEnumerator method)
    {
        AppInterMono app = AppInter.Instance;
        return app.StartCoroutine(method);
    }


    public static void WWWBundle(string url, Action<AssetBundle> finish)
    {
        StartCoroutine(wwwUtil<AssetBundle>(url , (obj)=> finish.Invoke((AssetBundle)obj)));
    }


    private static IEnumerator wwwUtil<T>(string url, Action<object> finish)
    {
        WWW _www = new WWW(url);
        yield return _www;

        if (_www.error != null)
        {
            Debug.LogError(_www.error + ",url:" + url);
            yield break;
        }

        Type type = typeof (T);
        if (type == typeof (AssetBundle))
        {
            AssetBundle bundle = _www.assetBundle;
            finish.Invoke(bundle);
        }else if (type == typeof (TextAsset))
        {
            finish.Invoke(_www.text);
        }
    } 
 
    
    public class AppInterMono : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);

            this.gameObject.name = "_App";
            this.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}