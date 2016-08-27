using System;
using System.Collections;

namespace NetCore
{
    public class ByteStream
    {
        protected ByteArray m_pBuffer;

        public byte readInt8()
        {
            return m_pBuffer.readInt8();
        }

        public void writeInt8(byte value)
        {
            m_pBuffer.writeInt8(value);
        }

        public Int16 readInt16()
        {
            return m_pBuffer.readInt16();
        }

        public void writeInt16(short value)
        {
            m_pBuffer.writeInt16(value);
        }

        public Int32 readInt32()
        {
            return m_pBuffer.readInt32();
        }

        public void writeInt32(int value)
        {
            m_pBuffer.writeInt32(value);
        }

        public Int64 readInt64()
        {
            return m_pBuffer.readInt64();
        }

        public void writeInt64(Int64 value)
        {
            m_pBuffer.writeInt64(value);
        }

        public string readString()
        {
            Int16 nStrLen = 0;

            nStrLen = readInt16();

            System.Diagnostics.Debug.Assert(m_pBuffer.dataSize() >= nStrLen);

            byte[] b = m_pBuffer.readBytes(nStrLen);

            string s = System.Text.Encoding.Default.GetString(b);
            return s;
        }

        public void writeString(string s)
        {
            short len = (short)s.Length;
            writeInt16(len);
            byte[] val = System.Text.Encoding.Default.GetBytes(s);
            m_pBuffer.writeBytes(val);
        }

        public ByteArray byteBuffer()
        {
            return m_pBuffer;
        }

        public void attachBuffer(ByteArray pBuffer)
        {
            if (m_pBuffer != null)
            {
                detachBuffer();
            }

            m_pBuffer = pBuffer;
        }

        public void detachBuffer()
        {
            m_pBuffer = null;
        }

    }
}
