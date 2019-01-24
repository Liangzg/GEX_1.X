/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GEX.Resource;
using RSG;

/// <summary>
/// 描述：UIPage管理器
/// <para>创建时间：2016-08-08</para>
/// </summary>
public class UIManager : ASingleton<UIManager>
{
    //页面缓存列表
    private Dictionary<string , UIPage> cachePagePool = new Dictionary<string, UIPage>();

    private List<UIPage> pageList = new List<UIPage>();

    protected EUIState curStateUI;
    protected EUIState lastStateUI;
    
    public EUIState CurStateUI
    {
        get { return curStateUI; }
    }

    private UIManager() { }

    /// <summary>
    /// Page GameObject Name 必须和LuaLogic的文件名一致，避免不必要的配置
    /// </summary>
    /// <param name="pagePath"></param>
    /// <param name="param"></param>
    public void Show(string pagePath, object param = null)
    {
        if (!cachePagePool.ContainsKey(pagePath))
        {
            new Promise<ALoadOperation>((s, j) =>
            {
                ALoadOperation loader = GResource.LoadBundleAsync(pagePath);
                loader.OnFinish += s;
                AppInter.StartCoroutine(loader);
            }).Then((loader) =>
            {
                GameObject prefab = loader.GetAsset<GameObject>();

                GameObject gObj = GameObject.Instantiate(prefab);
                gObj.name = Path.GetFileNameWithoutExtension(pagePath);

                LuaPageBehaviour pageBehaviour = gObj.AddComponent<LuaPageBehaviour>();
                UIPage page = new UIPage(pageBehaviour , pagePath);

                //缓存记录
                cachePagePool.Add(pagePath, page);
                showUIPage(page, param);

            }).Catch((e) =>
            {
                Debug.LogError("加载Page异常！ Path:" + pagePath);
                Debug.LogException(e);
            });
        }
        else
        {
            UIPage page = cachePagePool[pagePath];
            showUIPage(page, param);
        }
    }


    private void showUIPage(UIPage uiPage , object param)
    {
        //将新显示的Page添加到队尾
        pageList.Remove(uiPage);
        pageList.Add(uiPage);
        
        //界面适配
        adjustRoot(uiPage);

        uiPage.OnShow(param);
        
        checkPops(uiPage);
    }

    
    /// <summary>
    /// 检测并弹出不合格的数据
    /// </summary>
    /// <param name="page"></param>
    private void checkPops(UIPage page)
    {
        if (page.AttributePage.showMode != EShowMode.HideOther) return;
        
        //如果返回列表不为空，说明界面被反复显示调用，则添加一个分隔标识
        // 标识也可是使用Page的各页面配置，用于区分，因为异步情况下界面对象被释放无法使用
        int count = page.BackQueue.Count;
        if(count > 0 )    page.BackQueue.Add("_none_");
        
        for (int i = pageList.Count - 2; i >= 0; i--)
        {
            UIPage uiPage = pageList[i];

            if(uiPage.AttributePage.pageType == EPageType.Fixed || !uiPage.Active)    continue;
            
            uiPage.OnHide();
            pageList.RemoveAt(i);

            //记录上次显示的界面列表，用于返回操作
            //如果页面不需要返回时显示，则不用添加到返回列表
            if (uiPage.AttributePage.showMode != EShowMode.NoNeedBack)
            {
                page.BackQueue.Add(uiPage.AttributePage.assetPath);
            }
        }

        //如果没有新的返回列表，则不需要分隔标识
        if(count > 0 && count == page.BackQueue.Count - 1)  page.BackQueue.RemoveAt(count);
    }


    /// <summary>
    /// 适配根结点
    /// </summary>
    /// <param name="uiPage"></param>
    private void adjustRoot(UIPage uiPage)
    {
        UIPageRoot pageRoot = UIPageRoot.Instance;
        //设置界面的根结点
        Transform parentTrans = pageRoot.GetRoot(uiPage.AttributePage.pageType);
        RectTransform rectTrans = uiPage.gameObject.GetComponent<RectTransform>();
        rectTrans.SetParent(parentTrans);
        rectTrans.anchoredPosition = Vector2.zero;
        rectTrans.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 获得缓存PageUI
    /// </summary>
    /// <param name="pageName"></param>
    /// <returns></returns>
    public UIPage GetPageUI(string pageName)
    {
        if (!cachePagePool.ContainsKey(pageName)) return null;

        return cachePagePool[pageName];
    }
    
    /// <summary>
    /// 隐藏指定页面
    /// </summary>
    /// <param name="pageName">页面名</param>
    public void Hide(string pageName)
    {
        UIPage page = GetPageUI(pageName);
        if (page == null) return;

        for (int i = 0; i < pageList.Count; i++)
        {
            if (pageList[i] != page)    continue;

            for (int j = pageList.Count - 1; j > i; j--)
            {
                UIPage tPage = pageList[j];
                tPage.OnHide();
                pageList.RemoveAt(j);

                //记录上次显示的界面列表，用于返回操作
                //如果页面不需要返回时显示，则不用添加到返回列表
                if (tPage.AttributePage.showMode != EShowMode.NoNeedBack && 
                    tPage.AttributePage.showMode != EShowMode.HideOther)
                    page.BackQueue.Add(tPage.AttributePage.assetPath);
            }

            pageList.RemoveAt(i);
            break;
        }

        page.OnHide();
    }

    /// <summary>
    /// 自动返回上层界面
    /// </summary>
    public void AutoBackPage()
    {

        UIPage pageRoot = null;
        for (int i = pageList.Count - 1; i >= 0; i--)
        {
            UIPage page = pageList[i];

            //固定界面无法自动返回隐藏，需要强制
            //PopUp模式，会随父层的Normal界面关闭
            if(page.AttributePage.pageType == EPageType.Fixed || page.AttributePage.pageType == EPageType.PopUp)    continue;
            
            if (page.AttributePage.showMode != EShowMode.HideOther && page.AttributePage.showMode != EShowMode.None) continue;

            pageRoot = page;
            //隐藏当前的页面
            Hide(page.AttributePage.assetPath);
            break;
        }
        
        if (pageRoot == null) return;

        //返回上层界面
        List<string> backQueue = new List<string>();

        //返回队列被使用后清空
        string flag = "_none_";
        for (int i = pageRoot.BackQueue.Count - 1; i >= 0; i--)
        {
            if (pageRoot.BackQueue[i] == flag)
            {
                pageRoot.BackQueue.RemoveAt(i);
                break;
            }

            backQueue.Add(pageRoot.BackQueue[i]);
            pageRoot.BackQueue.RemoveAt(i);
        }
        
        foreach (string pagePath in backQueue)
        {
            Show(pagePath , null);
        }

    }

    /// <summary>
    /// 设置UI状态
    /// </summary>
    /// <param name="state"></param>
    public void SetStateUI(EUIState state)
    {
        Camera uiCam = UIPageRoot.Instance.uiCamera;

        uiCam.cullingMask = 0;
        curStateUI = state;

        switch (state)
        {
            case EUIState.Normal:
                CameraUtil.ShowLayerName(uiCam, "UI", "UIModel", "UIEffect", "TipsPenetrate", "GloableUI");
                break;
            case EUIState.Talk:
            case EUIState.Plot:
                CameraUtil.ShowLayerName(uiCam, "Plot", "GloableUI");
                break;
        }

    }
}
