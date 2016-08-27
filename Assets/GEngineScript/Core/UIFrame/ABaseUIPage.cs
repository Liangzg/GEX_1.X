/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using LuaInterface;
using UnityEngine.UI;

/// <summary>
/// 页面数据
/// </summary>
public class PageAttribute
{
    public EPageType PageType = EPageType.Normal;
    public EShowMode ShowMode = EShowMode.HideOther;
    public ECollider Collider = ECollider.Normal;
}

/// <summary>
/// 描述：UIPage基础抽象层
/// <para>创建时间：2016-08-08</para>
/// </summary>
public abstract class ABaseUIPage
{

    protected GameObject cacheGObj;

    protected Transform cacheTrans;

    protected PageAttribute mAttribute = new PageAttribute();

    private bool active = true;

    /// <summary>
    /// 返回列表
    /// </summary>
    public List<string> BackQueue = new List<string>();

    #region -----------Page 生命周期-----------------
    public virtual void OnAwake(GameObject gObj)
    {
        cacheGObj = gObj;
        cacheTrans = gObj.transform;
    }

    /// <summary>
    /// 每次显示时调用
    /// </summary>
    public virtual void OnShow(object param)
    {
        Active = true;
    }


    public virtual void OnHide()
    {
        Active = false;
    }


    public virtual void OnDestroy()
    {
    }

    #endregion

    public GameObject CacheGameObject
    {
        get { return cacheGObj; }
    }

    public Transform CacheTrans
    {
        get { return cacheTrans; }
    }
    /// <summary>
    /// 页面显示时配置属性
    /// </summary>
    public PageAttribute AttributePage
    {
        get { return mAttribute;}
    }

    /// <summary>
    /// 是否激活当前页面
    /// </summary>
    public bool Active
    {
        get { return active && cacheGObj.activeInHierarchy;}

        set
        {
            active = value;
            cacheGObj.SetActive(active);
        }
    }

    /// <summary>
    /// 设置页面显示配置属性
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="mode">显示模式</param>
    /// <param name="collider">碰撞模式</param>
    public void SetPage(EPageType type, EShowMode mode, ECollider collider)
    {
        mAttribute.PageType = type;
        mAttribute.ShowMode = mode;
        mAttribute.Collider = collider;
    }

    public string Name { get { return cacheGObj.name; } }
}



