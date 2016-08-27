/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// 描述：单例模板
/// <para>创建时间：</para>
/// </summary>
public abstract class ASignalEntry <T>
{

    protected static T mInstance;

    public static T Instance
    {
        get
        {
            if(mInstance != null)   return mInstance;
            mInstance = (T)Activator.CreateInstance(typeof(T) , BindingFlags.NonPublic|BindingFlags.Instance , null , null , null);
            return mInstance;
        }
    }

    
}
