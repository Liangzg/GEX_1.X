using UnityEngine;
using System;

namespace NetCore
{
    public class ByteArray
    {
        protected byte[] m_buffer = null;
        protected int m_nHeadOffset = 0;

        public byte[] readBytes(int nLen)
        {
            if (m_nHeadOffset + nLen > this.m_buffer.Length)
            {
                Debug.Log(" Failed: 长度越界  NetReader: getBYTE ");
                 throw new IndexOutOfRangeException(" Failed: 长度越界  NetReader: getBYTE ");
            }
            byte[] buf = new byte[nLen];
            Buffer.BlockCopy(m_buffer, m_nHeadOffset, buf, 0, nLen);
            this.m_nHeadOffset += nLen;
            return buf;
        }

        public void writeBytes(byte[] buffer)
        {
            if (buffer.Length < 1)
            {
                return;
            }

            if (m_buffer == null)
            {
                m_buffer = new byte[0];
            }
            byte[] buf = new byte[m_buffer.Length + buffer.Length];
            if (m_buffer.Length > 0)
            {
                Buffer.BlockCopy(m_buffer, 0, buf, 0, m_buffer.Length);
            }

            Buffer.BlockCopy(buffer, 0, buf, m_buffer.Length, buffer.Length);
            m_buffer = buf;
        }

        public int dataSize()
        {
            if (m_buffer == null)
            {
                return 0;
            }
            return m_buffer.Length - m_nHeadOffset;
        }

        public int dataPtr()
        {
            return m_nHeadOffset;
        }

        public string dataString()
        {
            return "";
        }
        
        //void resizeBuffer(int len);
        //void reserveBuffer(int len);
        public void consumeByte(int len)
        {
            System.Diagnostics.Debug.Assert(m_buffer.Length >= m_nHeadOffset + len);
            m_nHeadOffset += len;
        }

        public void clearBuffer()
        {
            m_buffer = new byte[0];

            m_nHeadOffset = 0;
        }

        sbyte byteAtIndex(int index)
        {
            if (index > m_buffer.Length)
            {
                return 0;
            }

            return Convert.ToSByte(m_buffer[index]);
        }

        public byte readInt8()
        {
            int nLen = sizeof(byte);
            if (this.m_nHeadOffset + nLen > this.m_buffer.Length)
            {
                Debug.Log(" Failed: 长度越界  NetReader: readInt8 ");
                throw new IndexOutOfRangeException(" Failed: 长度越界  NetReader: readInt8 ");
            }
            byte bt = m_buffer[m_nHeadOffset];
            this.m_nHeadOffset += nLen;
            return bt;
        }
        public short readInt16()
        {
            int nLen = sizeof(short);
            if (this.m_nHeadOffset + nLen > this.m_buffer.Length)
            {
                Debug.Log(" Failed: 长度越界  NetReader: readInt16 ");
                throw new IndexOutOfRangeException(" Failed: 长度越界  NetReader: readInt16 ");
            }
            short val = BitConverter.ToInt16(m_buffer, m_nHeadOffset);
            m_nHeadOffset += nLen;
            return val;

        }
        public int readInt32()
        {
            int nLen = sizeof(int);
            if (this.m_nHeadOffset + nLen > this.m_buffer.Length)
            {
                Debug.Log(" Failed: 长度越界  NetReader: readInt32 ");
                throw new IndexOutOfRangeException(" Failed: 长度越界  NetReader: readInt32 ");
            }
            int val = BitConverter.ToInt32(m_buffer, m_nHeadOffset);
            m_nHeadOffset += nLen;
            return val;
        }

        public long readInt64()
        {
            int nLen = sizeof(Int64);
            if (this.m_nHeadOffset + nLen > this.m_buffer.Length)
            {
                Debug.Log(" Failed: 长度越界  NetReader: readInt64 ");
                throw new IndexOutOfRangeException(" Failed: 长度越界  NetReader: readInt64 ");
            }
            long val = BitConverter.ToInt64(m_buffer, m_nHeadOffset);
            m_nHeadOffset += nLen;
            return val;
        }

        public void writeInt8(byte value)
        {
            byte[] val = new byte[1];
            val[0] = value;
            writeBytes(val);
        }

        public void writeInt16(short value)
        {
            byte[] val = BitConverter.GetBytes(value);
            writeBytes(val);
        }

        public void writeInt32(int value)
        {
            byte[] val = BitConverter.GetBytes(value);
            writeBytes(val);
        }

        public void writeInt64(long value)
        {
            byte[] val = BitConverter.GetBytes(value);
            writeBytes(val);
        }

        public void writeByteBuffer(ByteArray buffer)
        {
            writeBytes(buffer.m_buffer);
        }

        public ByteArray copyBufferPart(int startIndex, int nCount)
        {
            ByteArray buffer = new ByteArray();

            byte[] ByteArray = new byte[nCount];

            Buffer.BlockCopy(m_buffer, startIndex, ByteArray, 0, nCount);

            buffer.writeBytes(ByteArray);

            return buffer;

        }

        public ByteArray copyBufferAll()
        {
            ByteArray buffer = new ByteArray();

            int byteLen = dataSize() - dataPtr();

            byte[] ByteArray = new byte[byteLen];

            Buffer.BlockCopy(m_buffer, m_nHeadOffset, ByteArray, 0, byteLen);

            buffer.writeBytes(ByteArray);

            return buffer;
        }

        //void cleanHeadOffset();
        public byte[] getBuffer()
        {
            if (m_buffer !=null)
            {
                return m_buffer;
            }
            return new byte[0];
        }

    }
}


