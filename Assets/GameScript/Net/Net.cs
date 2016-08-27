using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using NetCore;
using LuaFramework;

enum KickCode
{
    /** 0 - 重复登录 */
    eDUPLICATE_LOGIN = 0,

    /** 1 - 玩家被封禁 */
    eBLOCK_LOGIN,

    /** 2 - 登录超时 */
    eLOGIN_TIMEOUT,
};

public enum NetErrorType
{
    eSendError = 0,	/*发送数据错误	-(重激活)*/
    eRecvError,			/*接受数据错误	-(重激活)*/
    eTransError			/*事务错误		-(重登录)*/
};

namespace NetCore
{
    public class Net
    {
        private Socket _socket;
        private Thread _thread = null;
        private const int TimeOut = 30;//30秒的超时时间
        private int _port;
        private string _ip;

        private readonly ByteArrayQueue _sendQueue;
        private readonly ByteArrayQueue _receiveQueue;

        public delegate void NetError();
        public NetError NetErrorCallback { get; set; }

        public Net()
        {
            _sendQueue = new ByteArrayQueue();
            _receiveQueue = new ByteArrayQueue();
        }

        public void setIp(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void startNet()
        {
            UnityEngine.NetworkReachability state = UnityEngine.Application.internetReachability;
            if (state != UnityEngine.NetworkReachability.NotReachable)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    _socket.Connect(_ip, _port);
                }
                catch (System.Exception)
                {
                    _socket = null;
                    throw;
                }

                _thread = new Thread(new ThreadStart(_threadEntry));
                _thread.Start();
            }

            //测试调用到lua
            //pushNetTask(onNetRecvError);
            //NetErrorCallback.Invoke();
        }

        public void shutdownNet()
        {
            if (_socket == null) return;
            try
            {
                lock (this)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;

                    _thread.Abort();
                    _thread = null;
                }
            }
            catch (System.Exception)
            {
                _socket = null;
            }
            
        }

        private void _threadEntry()
        {
            while (true)
            {
                if (_socket == null) 
                    return;

                _recvData();

                _sendData();

                Thread.Sleep(5);
            }
        }

        public void sendBytes(ByteArray buffer)
        {
            lock (_sendQueue)
            {
                _sendQueue.pushBack(buffer);
            }
        }

        private bool _sendData()
        {
            if (_socket != null)
            {
                ByteArray buffer;
                lock (_sendQueue)
                {
                    if (_sendQueue.empty()) return false;
                    buffer = _sendQueue.front();
                }
                byte[] data = buffer.getBuffer();
                try
                {
                    IAsyncResult asyncSend = _socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), _socket);
                    bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
                    if (!success)
                    {
                        shutdownNet();
                        return false;
                    }
                    else
                    {
                        _sendQueue.popFront();
                    }
                    return true;
                }
                catch (Exception)
                {
                    Debug.Log("_sendData wtf");
                }
            }
            return false;
        }

        private void sendCallback(IAsyncResult asyncSend)
        {

        }

        private void _recvData()
        {
            try
            {
                if (_socket.Poll(5, SelectMode.SelectRead))
                {
                    byte[] prefix = new byte[4]; //包长度
                    int recnum = _socket.Receive(prefix);
                    if (recnum == 4)
                    {

                        int len = BitConverter.ToInt32(prefix,0);
                        //int l = Endian.s
                        int datalen = Endian.SwapInt32(len) - 4;
                        byte[] data = new byte[datalen];
                        int startIndex = 0;
                        recnum = 0;
                        do 
                        {
                            int rev = _socket.Receive(data, startIndex, datalen - recnum, SocketFlags.None);
                            recnum += rev;
                            startIndex += rev;

                        } while (recnum != datalen);
                        ByteArray buffer = new ByteArray();
                        //buffer.WriteBytes(prefix);
                        buffer.writeBytes(data);
                        lock (_receiveQueue)
                        {
                            _receiveQueue.pushBack(buffer);
                        }
                            
                    }
                }
                else if (_socket.Poll(5, SelectMode.SelectError))
                {
                    shutdownNet();
                    UnityEngine.Debug.Log("SelectError Close Socket");
                }
            }
            catch (Exception)
            {
                Debug.Log("_recvData wtf");
            }
        }


        public ByteArray pickRevQueue()
        {
            ByteArray byteBuffer = null;
            lock (_receiveQueue)
            {
               byteBuffer = _receiveQueue.frontAndPop();
            }

            return byteBuffer;
        }

        public void pushNetTask(NetError netError)
        {
            NetErrorCallback += netError;
        }

        public void onNetRecvError()
        {
	        notifyError(NetErrorType.eRecvError);
        }

        public void onNetSendError()
        {
	        notifyError(NetErrorType.eRecvError);
        }

        public void onNetTransError()
        {
	        notifyError(NetErrorType.eRecvError);
        }

        public void notifyError(NetErrorType errorType)
        {
            object[] obj = {errorType,};
            CallMethod("onSocketError", obj);
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        public object[] CallMethod(string func, params object[] args)
        {
            //return Util.CallMethod("NetClient", func, args);
            return null;
        }
    }
}
