/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：打包策略统一接口
/// <para>创建时间：</para>
/// </summary>
public interface IStrategy
{
    /// <summary>
    /// 开始流程
    /// </summary>
    void BeginProcess(BuildConfig buildConfig);

    /// <summary>
    /// 结束流程
    /// </summary>
    void EndProcess(BuildConfig buildConfig);

}
