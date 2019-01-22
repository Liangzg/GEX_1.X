/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaFramework;

/// <summary>
/// 描述：UIPage管理器
/// <para>创建时间：2016-08-08</para>
/// </summary>
public class UIManager : ASingleton<UIManager>
{
    //页面缓存列表
    private Dictionary<string , ABaseUIPage> cachePagePool = new Dictionary<string, ABaseUIPage>();

    private List<ABaseUIPage> pageList = new List<ABaseUIPage>();
    
    private UIManager() { }

    /// <summary>
    /// Page GameObject Name 必须和LuaLogic的文件名一致，避免不必要的配置
    /// </summary>
    /// <param name="luaLogic"></param>
    /// <param name="param"></param>
    public void Show(string luaLogic , object param = null)
    {
        string fileName = luaLogic;
        if (!fileName.EndsWith(".lua")) fileName += ".lua";

        //加载Lua逻辑
        BaseLuaManager luaMgr = AppFacade.Instance.GetManager<BaseLuaManager>(ManagerName.Lua);
        luaMgr.DoFile(fileName);

        //加载界面资源
        string resName = Path.GetFileNameWithoutExtension(fileName);
        resName += ".prefab";
        ShowPageName(resName , param);
    }


    /// <summary>
    /// 根据页面的名字显示
    /// </summary>
    /// <param name="pageName">页面的名字</param>
    /// <param name="param"></param>
    public void ShowPageName(string pageName, object param)
    {
        string tPageName = Path.GetFileNameWithoutExtension(pageName);
        if (!cachePagePool.ContainsKey(tPageName))
        {
            AssetLoader.LoadUI(pageName, gObj =>
            {
                if (gObj == null) return;

                gObj.name = tPageName;
                LuaUIPage page = new LuaUIPage();
                page.OnAwake(gObj);

                //缓存记录
                cachePagePool.Add(page.Name, page);
                showUIPage(page, param);
            });
        }
        else
        {
            ABaseUIPage page = cachePagePool[tPageName];
            showUIPage(page, param);
        }
    }


    private void showUIPage(ABaseUIPage uiPage , object param)
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
    private void checkPops(ABaseUIPage page)
    {
        if (page.AttributePage.ShowMode != EShowMode.HideOther) return;
        
        //如果返回列表不为空，说明界面被反复显示调用，则添加一个分隔标识
        // 标识也可是使用Page的各页面配置，用于区分，因为异步情况下界面对象被释放无法使用
        int count = page.BackQueue.Count;
        if(count > 0 )    page.BackQueue.Add("_none_");
        
        for (int i = pageList.Count - 2; i >= 0; i--)
        {
            ABaseUIPage uiPage = pageList[i];

            if(uiPage.AttributePage.PageType == EPageType.Fixed || !uiPage.Active)    continue;
            
            uiPage.OnHide();
            pageList.RemoveAt(i);

            //记录上次显示的界面列表，用于返回操作
            //如果页面不需要返回时显示，则不用添加到返回列表
            if (uiPage.AttributePage.ShowMode != EShowMode.NoNeedBack)
            {
                page.BackQueue.Add(uiPage.Name);
            }
        }

        //如果没有新的返回列表，则不需要分隔标识
        if(count > 0 && count == page.BackQueue.Count - 1)  page.BackQueue.RemoveAt(count);
    }


    /// <summary>
    /// 适配根结点
    /// </summary>
    /// <param name="uiPage"></param>
    private void adjustRoot(ABaseUIPage uiPage)
    {
        //设置界面的根结点
        Transform parentTrans = UIPageRoot.Instance.GetRoot(uiPage.AttributePage.PageType);
        RectTransform rectTrans = uiPage.CacheGameObject.GetComponent<RectTransform>();
        rectTrans.SetParent(parentTrans);
        rectTrans.anchoredPosition = Vector2.zero;
        rectTrans.transform.localScale = Vector3.one;

    }

    /// <summary>
    /// 获得缓存PageUI
    /// </summary>
    /// <param name="pageName"></param>
    /// <returns></returns>
    public ABaseUIPage GetPageUI(string pageName)
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
        ABaseUIPage page = GetPageUI(pageName);
        if (page == null) return;

        for (int i = 0; i < pageList.Count; i++)
        {
            if (pageList[i] != page)    continue;

            for (int j = pageList.Count - 1; j > i; j--)
            {
                ABaseUIPage tPage = pageList[j];
                tPage.OnHide();
                pageList.RemoveAt(j);

                //记录上次显示的界面列表，用于返回操作
                //如果页面不需要返回时显示，则不用添加到返回列表
                if (tPage.AttributePage.ShowMode != EShowMode.NoNeedBack && 
                    tPage.AttributePage.ShowMode != EShowMode.HideOther)
                    page.BackQueue.Add(tPage.Name);
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

        ABaseUIPage pageRoot = null;
        for (int i = pageList.Count - 1; i >= 0; i--)
        {
            ABaseUIPage page = pageList[i];

            //固定界面无法自动返回隐藏，需要强制
            //PopUp模式，会随父层的Normal界面关闭
            if(page.AttributePage.PageType == EPageType.Fixed || page.AttributePage.PageType == EPageType.PopUp)    continue;
            
            if (page.AttributePage.ShowMode != EShowMode.HideOther && page.AttributePage.ShowMode != EShowMode.None) continue;

            pageRoot = page;
            //隐藏当前的页面
            Hide(page.Name);
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
        
        foreach (string pageName in backQueue)
        {
            ShowPageName(pageName , null);
        }

    }
}
