/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 描述：UIPage基础抽象层
/// <para>创建时间：2016-08-08</para>
/// </summary>
public class UIPage
{

    protected GameObject cacheGameObj;

    protected Transform cacheTrans;

    protected PageContext context;

    private bool active = true;

    /// <summary>
    /// 返回列表
    /// </summary>
    public List<string> BackQueue = new List<string>();

    protected UIPageRoot pageRoot;

    protected LuaPageBehaviour luaBehaviour;
    
    public GameObject gameObject
    {
        get { return this.cacheGameObj; }
    }

    #region -----------Page 生命周期-----------------
    public UIPage(LuaPageBehaviour luaBehaviour , string pagePath)
    {
        pageRoot = UIPageRoot.Instance;
        context = new PageContext();

        this.luaBehaviour = luaBehaviour;

        context.assetPath = pagePath;
        context.pageName = Path.GetFileNameWithoutExtension(pagePath);

        cacheGameObj = luaBehaviour.gameObject;
        cacheTrans = cacheGameObj.transform;
    }

    /// <summary>
    /// 每次显示时调用
    /// </summary>
    public void OnShow(object param)
    {
        Active = true;

        this.luaBehaviour.OnShow(this, param);

        Canvas canvas = cacheGameObj.GetComponent<Canvas>();
        if (AttributePage.pageType != EPageType.Fixed)
            canvas.sortingOrder = pageRoot.AddOrder(AttributePage.pageType);
        else
            canvas.sortingOrder = AttributePage.order;

    }


    public virtual void OnHide()
    {
        Active = false;

        if (AttributePage.pageType != EPageType.Fixed)
            pageRoot.SubOrder(AttributePage.pageType);

        this.luaBehaviour.OnHide();
    }


    #endregion

    
    public Transform CacheTrans
    {
        get { return cacheTrans; }
    }
    /// <summary>
    /// 页面显示时配置属性
    /// </summary>
    public PageContext AttributePage
    {
        get { return context;}
    }

    /// <summary>
    /// 是否激活当前页面
    /// </summary>
    public bool Active
    {
        get { return active && cacheGameObj.activeInHierarchy;}

        set
        {
            active = value;
            cacheGameObj.SetActive(active);
        }
    }

    /// <summary>
    /// 设置界面显示配置属性
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="mode">显示模式</param>
    /// <param name="collider">碰撞模式</param>
    public void SetPage(EPageType type, EShowMode mode, ECollider collider)
    {
        
        context.pageType = type;
        context.showMode = mode;
        context.collider = collider;
    }

    /// <summary>
    /// 设置固定界面的属性
    /// </summary>
    /// <param name="order"></param>
    /// <param name="mode"></param>
    /// <param name="collider"></param>
    public void SetFixPage(int order, EShowMode mode, ECollider collider)
    {
        context = new PageContext();
        context.pageType = EPageType.Fixed;
        context.order = order;
        context.showMode = mode;
        context.collider = collider;
    }

    public string Name { get { return cacheGameObj.name; } }
}



