/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：页面运行环境的配置
/// </summary>
public class PageContext
{
    /// <summary>
    /// 匹配的显示状态
    /// </summary>
    public EUIState uiState = EUIState.Normal;
    /// <summary>
    /// 界面类型
    /// </summary>
    public EPageType pageType = EPageType.Normal;
    /// <summary>
    /// 显示交互模式
    /// </summary>
    public EShowMode showMode = EShowMode.HideOther;
    /// <summary>
    /// 背景碰撞方式
    /// </summary>
    public ECollider collider = ECollider.Normal;
    /// <summary>
    /// 界面显示序列
    /// </summary>
    public int order;
    /// <summary>
    /// 界面名称
    /// </summary>
    public string pageName;
    /// <summary>
    /// 资源路径
    /// </summary>
    public string assetPath;
    /// <summary>
    /// Lua逻辑
    /// </summary>
    public LuaPageBehaviour luaPage;
}
