using UnityEngine;
using System.Collections;

namespace GEX.Network
{

    public enum SocketState
    {
        Connecting,      //连接中
        Connected,       //已连接
        Disconnecting,   //断开中
        Disconnected,    //已断开
    }

}
