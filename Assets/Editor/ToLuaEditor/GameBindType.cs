/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using NetCore;
using LuaFramework;

/// <summary>
/// 描述：具体游戏类型绑定
/// <para>创建时间：2016-08-03</para>
/// </summary>
public sealed class GameBindType
{
    /// <summary>
    /// 游戏功能脚本Wrap绑定块
    /// </summary>
    public static ToLuaMenu.BindType[] GameScripteBinds = new[]
    {
        //网络
        _GT<ByteArray>(),
        _GT<ByteArrayQueue>(),
        _GT<ByteStream>(),
        _GT<Net>(),
        _GT<PackProtocol>(),
        _GT<VarintStream>(),

        //工具
        _GT<Util>(),
    };

    /// <summary>
    /// 游戏引擎脚本Wrap绑定块
    /// </summary>
    public static ToLuaMenu.BindType[] GameEngineBinds = new[]
    {
        _GT<AssetLoader>(),
        _GT<LuaAssetLoader>(),

        _GT<LuaUIPage>(),
        _GT<ABaseUIPage>(),
        _GT<EPageType>(),
        _GT<EShowMode>(),
        _GT<ECollider>(),
        _GT<UIManager>(),
    };

    /// <summary>
    /// UnityEngine脚本绑定块
    /// </summary>
    public static ToLuaMenu.BindType[] UnityEngineBinds = new []
    {
        #if UNITY_EDITOR
        _GT<GUILayout>(),

	    #endif
        _GT<UnityEngine.UI.Text>(),
        _GT<RectTransform>(),

    };


    
    public static ToLuaMenu.BindType _GT<T>()
    {
        return new ToLuaMenu.BindType(typeof(T));
    }

    public static ToLuaMenu.BindType _GT(Type type)
    {
        return new ToLuaMenu.BindType(type);
    }
}
