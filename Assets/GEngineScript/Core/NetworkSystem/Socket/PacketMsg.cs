using System;
using System.IO;

namespace GEX.Network
{
    /// <summary>
    /// 消息解包
    /// </summary>
    public sealed class PacketMsg
    {
        public enum PacketReadState
        {
            WAIT_NEW_PACKET,
            WAIT_DATA_SIZE,
            WAIT_DATA_SIZE_FRAGMENT,
            WAIT_DATA,
            INVALID_DATA,
        }

        private MemoryStream packetStream ;
        private static readonly int INT_BYTE_SIZE = 4;
        private int _expectedLength = -1;
        private PacketReadState readState;
        private int _skipBytes;

        public Action<byte[]> OnBroadcastPacket; 

        public PacketMsg()
        {
            packetStream = new MemoryStream();
            readState = PacketReadState.WAIT_NEW_PACKET;
        }

        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="msgs"></param>
        public void ParsePacketMsg(byte[] msgs)
        {
            if (msgs.Length <= 0)
                throw new Exception("Unexpected empty packet msgs: no readable bytes available!");

            ByteArray data = new ByteArray(msgs);

            while (data.BytesAvailable > 0)
            {
                if (this.readState == PacketReadState.WAIT_NEW_PACKET)
                    this.readNewPacket(data);
                else if (this.readState == PacketReadState.WAIT_DATA_SIZE)
                    this.readMsgSize(data);
                else if (this.readState == PacketReadState.WAIT_DATA_SIZE_FRAGMENT)
                    this.readMsgFragment(data);
                else if (this.readState == PacketReadState.WAIT_DATA)
                    this.readPacketData(data);
                else if (this.readState == PacketReadState.INVALID_DATA)
                    this.readInvalidData(data);
            }
        }

        /// <summary>
        /// 开始读取新包
        /// </summary>
        /// <param name="data"></param>
        private void readNewPacket(ByteArray data)
        {
            this.packetStream.Seek(0, SeekOrigin.Begin);
            this.packetStream.SetLength(0);

            this._expectedLength = -1;
            this.readState = PacketReadState.WAIT_DATA_SIZE;
        }

        /// <summary>
        /// 读取数据的大小
        /// </summary>
        /// <param name="msg"></param>
        private void readMsgSize(ByteArray msg)
        {
            int num = msg.ReadInt();
            int pos = PacketMsg.INT_BYTE_SIZE;
            if (num != -1)
            {
                this._expectedLength = num;
                resizeByteArray(msg , msg.Length - pos);
                this.readState = PacketReadState.WAIT_DATA;
                return;
            }
            this.readState = PacketReadState.WAIT_DATA_SIZE_FRAGMENT;
            resizeByteArray(msg, msg.Length);
        }

        /// <summary>
        /// 读取消息的数据片段
        /// </summary>
        /// <param name="msg"></param>
        private void readMsgFragment(ByteArray msg)
        {
            int num1 = PacketMsg.INT_BYTE_SIZE - (int)this.packetStream.Length;
            if (msg.Length >= num1)
            {
                resizeByteArray(msg,  num1);
                
                if (msg.Length > num1)
                {      
                    resizeByteArray(msg , msg.Length - num1);
                }

                this.readState = PacketReadState.WAIT_DATA;
                return ;
            }

            resizeByteArray(msg, msg.Length);
        }

        /// <summary>
        /// 非法数据读取
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void readInvalidData(ByteArray data)
        {
            if (this._skipBytes == 0)
            {
                this.readState = PacketReadState.WAIT_NEW_PACKET;
                return ;
            }
            int pos = Math.Min(data.Length, this._skipBytes);
            resizeByteArray(data , data.Length - pos);
            this._skipBytes = this._skipBytes - pos;
        }


        private void readPacketData(ByteArray data)
        {
            int num = this._expectedLength - (int)this.packetStream.Length;
            bool flag = data.Length >= num;
            try
            {
                if (flag)
                {
                    resizeByteArray(data, num);
                    this.packetStream.Flush();

                    this.dispatchRequest(this.packetStream);
                    this.readState = PacketReadState.WAIT_NEW_PACKET;

                    resizeByteArray(data, data.Length - num);
                }
                else
                    resizeByteArray(data, data.Length);
            }
            catch (Exception ex)
            {
                Debugger.LogException("Error handling data ", ex);
                this._skipBytes = num;
                this.readState = PacketReadState.INVALID_DATA;
            }
        }


        private void dispatchRequest(MemoryStream stream)
        {
//            IMPObject mpObject = (IMPObject)MPObject.NewFromBinaryData(_buffer);
//            Packet packet = new Packet();
//            if (mpObject.IsNull("a"))
//                throw new Exception("Request rejected: No Action ID in request!");
//            packet.ActionID = mpObject.GetByte("a");
//            packet.Parameters = mpObject.GetMPObject("p");
//            this._controller.HandlePacket(packet);
            

            byte[] packetMsgs = stream.ToArray();
            OnBroadcastPacket.Invoke(packetMsgs);
            
        }


        private void resizeByteArray(ByteArray data, int len)
        {
            if (len <= 0) return;

            byte[] buf = data.ReadBytes(len);
            packetStream.Write(buf , 0 , buf.Length);
        }
    }
}

