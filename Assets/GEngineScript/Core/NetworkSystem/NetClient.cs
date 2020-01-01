using UnityEngine;
using System.Collections;

namespace GEX.Network
{
    /// <summary>
    /// 客户端网络层主入口操作
    /// </summary>
    public sealed class NetClient
    {
        private SocketTcp socket;

        private PacketMsg packetConvert;

        private ConnectObservice connectObserver;

        private HeartBeatService heartBeatService;

        #region ---Public Attributes-----

        public PacketMsg PacketConvert
        {
            get { return packetConvert; }
        }

        public ConnectObservice ConnectObserver
        {
            get { return connectObserver; }
        }

        /// <summary>
        /// 是否进入省电模式
        /// </summary>
        public bool IsPowerSaveMode { get; set; }

        #endregion

        public NetClient()
        {
            socket = new SocketTcp();
            socket.OnReceivedMsg += onReceiveMsg;

            packetConvert = new PacketMsg();
            packetConvert.OnBroadcastPacket += onBrodcastPacket;

            connectObserver = new ConnectObservice();

            heartBeatService = new HeartBeatService(this);
        }


        public void Connect(string serverIp, ushort port)
        {
            socket.Connect(serverIp, port);
        }


        public void Disconnect()
        {
            
        }

        private void onReceiveMsg(byte[] msgs)
        {
            packetConvert.ParsePacketMsg(msgs);

            heartBeatService.ResetTimeout();
            
        }

        /// <summary>
        /// 协议包广播
        /// </summary>
        /// <param name="packet"></param>
        private void onBrodcastPacket(byte[] packet)
        {
            ByteArray buffer = new ByteArray(packet);

            //读取特殊协议号，比如心跳
//            short pbId = buffer.ReadShort();
//            if (pbId == 10000)
//            {
//                //假如心跳ID是10000,这里要改成变量
//                heartBeatService.OnPingUpdate();
//            }
        }


        public void OnUpdate()
        {
            //heartBeatService.OnUpdate();
        }
        /// <summary>
        /// 发送请求信息
        /// </summary>
        /// <param name="msgs"></param>
        public void Send(byte[] msgs)
        {
            socket.Send(msgs);
        }
    }

}

