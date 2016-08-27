using System;
using System.Collections.Generic;

namespace NetCore
{
    public class PackProtocol
    {
        protected short m_sn = 0;

        public ByteArray signByteStream(byte module, byte method, byte[] data)
        {
            //* <p>[4字节] 长度
            //* <p>[1字节] 校验码
            //* <p>[1字节] 循环顺序号(从0开始,每次请求+1,不符合则断开; 0,1,2,...,127,-128,-127,...,-1,0,1,...)
            //* <p>[1字节] 模块号
            //* <p>[1字节] 命令号
            //包头长度 8 字节
            int len = data.Length + 8;
            short validateCode = fnvhash(m_sn, module, method, len);

            ByteArray byteBuffer = new ByteArray();
            byteBuffer.writeInt32(Endian.SwapInt32(len));
            byteBuffer.writeInt8((byte)validateCode);
            byteBuffer.writeInt8((byte)m_sn);
            byteBuffer.writeInt8(module);
            byteBuffer.writeInt8(method);
            byteBuffer.writeBytes(data);

            m_sn++;

            return byteBuffer;
        }

        public ByteArray signByteStream(ByteArray byteBuffer)
        {
            //* <p>[4字节] 长度
            //* <p>[1字节] 校验码
            //* <p>[1字节] 循环顺序号(从0开始,每次请求+1,不符合则断开; 0,1,2,...,127,-128,-127,...,-1,0,1,...)
            //* <p>[1字节] 模块号
            //* <p>[1字节] 命令号
            //包头长度 8 字节
            //self.netSendPackVStream:writeInt8(packet.req['_MOD_'])
            //self.netSendPackVStream:writeInt8(packet.req['_MED_'])
            //self.netSendPackVStream:writeInt32(0)--预留六字节
            //self.netSendPackVStream:writeInt16(0)
            byte[] data = byteBuffer.getBuffer();
            int len = data.Length;
            byte module = byteBuffer.readInt8();
            byte method = byteBuffer.readInt8();
            short validateCode = fnvhash(m_sn, module, method, len);

            ByteArray buf = new ByteArray();
            buf.writeInt32(Endian.SwapInt32(len));
            buf.writeInt8((byte)validateCode);
            buf.writeInt8((byte)m_sn);
            buf.writeInt8(module);
            buf.writeInt8(method);

            byte[] bytes = new byte[len - 8];
            Buffer.BlockCopy(data, 8, bytes, 0, len - 8);
            buf.writeBytes(bytes);

            m_sn++;

            return buf;
        }

        public void resetSN()
        {
            m_sn = 0;
        }

        private short fnvhash(short sn, short module, short cmd, Int32 len)
        {
            int hash = 117;
            int prime = 101;
            int lenLow = len & 0xFFFF;
            int lenHight = (int)(len & 0xFFFF0000) >> 16;
            hash = (hash ^ sn) * prime;
            hash = (hash ^ module) * prime;
            hash = (hash ^ cmd) * prime;
            hash = (hash ^ lenLow) * prime;
            hash = (hash ^ lenHight) * prime;
            return (short)(hash & 0xFF);
        }
    }
}
