using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GEX.Network
{
    /// <summary>
    /// Socket TCP协议操作
    /// </summary>
    public sealed class SocketTcp
    {

        private SocketState socketState = SocketState.Disconnected;

        private string serverAddress;
        private int serverPort;

        private Socket socket;
        private Thread socketThread;

        private const int MAX_TIMEOUT = 15000;
        private const int MAX_BYTE_BUFFER = 4096;

        private byte[] byteBuffer = new byte[MAX_BYTE_BUFFER];

        private readonly object _syncer = new object();

        public Action<byte[]> OnReceivedMsg; 

        #region ------Public Attributes---------

        public SocketState SocketState
        {
            get { return socketState; }
        }

        /// <summary>
        /// 是否已成功连接，true表示已连接
        /// </summary>
        public bool IsConnected
        {
            get { return socketState == SocketState.Connected; }
        }

        #endregion

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect(string serverIp , ushort port)
        {
            if (this.socketState != SocketState.Disconnected)
            {
                Debugger.LogError("Calling connect when the socket is not disconnected");
                return true;
            }

            this.socketState = SocketState.Connecting;
            this.serverAddress = serverIp;
            this.serverPort = port;

            this.onConnectStart();

            return false;
        }

        /// <summary>
        /// 执行启动Socket连接
        /// </summary>
        private void onConnectStart()
        {
            try
            {
                IPAddress ipAddress = getIpAddress(this.serverAddress);
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.SendTimeout = MAX_TIMEOUT;
                socket.NoDelay = true;
                this.socket.Connect(ipAddress, this.serverPort);

                this.socketState = SocketState.Connected;
            }
            catch (Exception)
            {
                throw;
            }

            //启动socket连接线程
            socketThread = new Thread(new ThreadStart(this.onThread));
            socketThread.IsBackground = true;
            socketThread.Start();

        }

        /// <summary>
        /// 获得IP Address的类型
        /// </summary>
        /// <param name="serverIp"></param>
        /// <returns></returns>
        protected IPAddress getIpAddress(string serverIp)
        {
            IPAddress address = (IPAddress)null;
            IPAddress ipAddress;
            if (IPAddress.TryParse(serverIp, out address))
            {
                ipAddress = address;
            }
            else
            {
                IPAddress[] addressList = Dns.GetHostEntry(serverIp).AddressList;
                foreach (IPAddress ipAddress2 in addressList)
                {
                    if (ipAddress2.AddressFamily == AddressFamily.InterNetwork)
                        return ipAddress2;
                }
                foreach (IPAddress ipAddress2 in addressList)
                {
                    if (ipAddress2.AddressFamily == AddressFamily.InterNetworkV6)
                        return ipAddress2;
                }
                ipAddress = (IPAddress)null;
            }
            return ipAddress;
        }

        /// <summary>
        /// 主动断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (this.socketState == SocketState.Disconnected)
                return false;

            this.socketState = SocketState.Disconnecting;

            lock (_syncer)
            {
                if (this.socketThread != null)
                    this.socketThread.Abort();
                this.socketThread = null;

                if (this.socket != null)
                {
                    try
                    {
                        this.socket.Close();
                    }
                    catch (Exception e)
                    {
                        Debugger.LogException("Socket Tcp disconnect error " , e);
                    }
                    socket = null;
                }

                this.socketState = SocketState.Disconnected;
            }
            
            return true;
        }

        /// <summary>
        /// 发送请求到服务器
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns></returns>
        public bool Send(byte[] msgs)
        {
            if (this.socketState != SocketState.Connected)
            {
                Debugger.LogError("Trying to write to disconnected socket");
                return false;
            }

            
            try
            {
                socket.Send(msgs);
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return true;
        }


        private void onThread()
        {
            int size = 0;
            try
            {
                while (socketState == SocketState.Connected)
                {
                    size = this.socket.Receive(this.byteBuffer, 0, MAX_BYTE_BUFFER, SocketFlags.None);
                    if (size <= 0)
                        throw new SocketException(10054);

                    byte[] msgs = new byte[size];
                    Buffer.BlockCopy(byteBuffer, 0, msgs, 0, size);

                    this.onReceiveMsgs(msgs);


                }
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        /// <summary>
        /// 接收读取数据
        /// </summary>
        /// <param name="msgs"></param>
        private void onReceiveMsgs(byte[] msgs)
        {
            
            //传递到数据解析层
            OnReceivedMsg.Invoke(msgs);
        }
    }

}


