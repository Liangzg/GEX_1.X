/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

/// 描述：UI框架枚举集合
/// <para>创建时间：2016-08-08</para>


//页面主体类型
public enum EPageType
{
    Fixed,  //固定页面 ， 比如固定的toolbar
    Normal, //常规页面
    PopUp, //模式窗口 , 最上层的页面，比如：消息盒，确认窗口
}


/// <summary>
/// 显示模式
/// </summary>
public enum EShowMode
{
    None , HideOther ,
    NeedBack,      // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
    NoNeedBack,    // 关闭TopBar,关闭其他界面,不加入backSequence队列
}

/// <summary>
/// 碰撞遮挡
/// </summary>
public enum ECollider
{
    None,      // 显示该界面不包含碰撞背景
    Normal,    // 碰撞透明背景
    WithBg,    // 碰撞非透明背景
}

/// <summary>
/// UI渲染状态,不同状态对应不同的Layer
/// </summary>
public enum EUIState
{
    Normal , //正常UI状态
    Talk,   //对话状态
    Plot,   //剧情动画
}