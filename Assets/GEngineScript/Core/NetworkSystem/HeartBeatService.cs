using System;
using UnityEngine;
using System.Collections;
using System.Threading;

namespace GEX.Network
{
    /// <summary>
    /// 心跳服务
    /// </summary>
    public class HeartBeatService
    {
        private int interval = 5000; //5秒

        private int timeout; //超时临界值
        private float elaspedTime;

        private Timer timer;  //计时器

        private DateTime lastTime;

        private const float HEARTBEAT_DURATION = 150;  //

        private static readonly byte[] m_lockObject = new byte[0] {};

        private NetClient netClient;

        public Action OnLostConnect;

        #region ---Public Attributes---
        /// <summary>
        /// 是否正在进行心跳
        /// <returns>true表示心跳服务正在执行</returns>
        /// </summary>
        public bool IsBeating
        {
            get { return timer != null; }
        }

        #endregion

        public HeartBeatService(NetClient net)
        {
            this.netClient = net;

            
        }
        
        // Use this for initialization
        public void OnStart()
        {
            this.timer = new Timer(checkHeartBeat, null, 0, interval);

            timeout = 0;
            elaspedTime = HEARTBEAT_DURATION;
            lastTime = DateTime.Now;
        }

        /// <summary>
        /// 重置超时
        /// </summary>
        public void ResetTimeout()
        {
            lock (m_lockObject)
            {
                timeout = 0;
                lastTime = DateTime.Now;
            }
        }

        private void checkHeartBeat(object obj)
        {
            lock (m_lockObject)
            {
                TimeSpan span = DateTime.Now - lastTime;
                timeout = (int)span.TotalMilliseconds;

                //check timeout
                if (timeout > interval * (netClient.IsPowerSaveMode ? 60 : 6))
                {
                    if (OnLostConnect != null)  OnLostConnect.Invoke();
                }
            }
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            elaspedTime -= Time.deltaTime;

            if (elaspedTime <= 0)
            {
                elaspedTime = HEARTBEAT_DURATION;
                netClient.ConnectObserver.NotifyEvent(ProtocalEvent.HeartBeat2Server , null);
            }
        }


        public void OnPingUpdate()
        {
            ushort protocal = ProtocalEvent.PingUpdate;
            ByteArray buffer = new ByteArray();
            buffer.WriteShort((short)protocal);
            netClient.ConnectObserver.NotifyEvent(protocal , buffer);
        }


        public void Stop()
        {
            if (timer != null)
                timer.Dispose();
            timer = null;

            elaspedTime = 0;
        }
        
    }

}

