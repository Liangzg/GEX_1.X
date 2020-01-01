using System;
using UnityEngine;
using System.Collections;

namespace GEX.Network
{
    public class ByteArray
    {
        private byte[] buffer;
        private int position;

        public byte[] Bytes
        {
            get
            {
                return this.buffer;
            }
            set
            {
                this.buffer = value;
            }
        }

        public int BytesAvailable
        {
            get
            {
                int num = this.buffer.Length - this.position;
                if (num <= this.buffer.Length && num >= 0)
                    return num;
                return 0;
            }
        }

        public int Length
        {
            get
            {
                return this.buffer.Length;
            }
        }

        public int Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public ByteArray()
        {
            this.position = 0;
            this.buffer = new byte[0];
        }

        public ByteArray(byte[] buf)
        {
            this.position = 0;
            this.buffer = buf;
        }

        public bool ReadBool()
        {
            byte[] numArray = this.buffer;
            int num = this.position;
            this.position = num + 1;
            int index = num;
            return (int)numArray[index] == 1;
        }

        public byte ReadByte()
        {
            byte[] numArray = this.buffer;
            int num = this.position;
            this.position = num + 1;
            int index = num;
            return numArray[index];
        }

        public byte[] ReadBytes(int count)
        {
            byte[] numArray = new byte[count];
            Buffer.BlockCopy((Array)this.buffer, this.position, (Array)numArray, 0, count);
            this.position = this.position + count;
            return numArray;
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(this.ReverseOrder(this.ReadBytes(8)), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(this.ReverseOrder(this.ReadBytes(4)), 0);
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(this.ReverseOrder(this.ReadBytes(4)), 0);
        }

        public long ReadLong()
        {
            return BitConverter.ToInt64(this.ReverseOrder(this.ReadBytes(8)), 0);
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(this.ReverseOrder(this.ReadBytes(2)), 0);
        }

        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(this.ReverseOrder(this.ReadBytes(2)), 0);
        }

        public string ReadUTF()
        {
            ushort num = this.ReadUShort();
            if ((int)num == 0)
                return (string)null;
            string @string = System.Text.Encoding.UTF8.GetString(this.buffer, this.position, (int)num);
            this.position = this.position + (int)num;
            return @string;
        }

        public byte[] ReverseOrder(byte[] dt)
        {
            if (!BitConverter.IsLittleEndian)
                return dt;
            byte[] numArray = new byte[dt.Length];
            int num = 0;
            for (int index = dt.Length - 1; index >= 0; --index)
                numArray[num++] = dt[index];
            return numArray;
        }

        public void WriteBool(bool b)
        {
            this.WriteBytes(new byte[1]{(byte) (!b ? 0 : 1)});
        }

        public void WriteByte(byte b)
        {
            this.WriteBytes(new byte[1]{b});
        }

        public void WriteBytes(byte[] data)
        {
            this.WriteBytes(data, 0, data.Length);
        }

        public void WriteBytes(byte[] data, int ofs, int count)
        {
            byte[] numArray = new byte[count + this.buffer.Length];
            Buffer.BlockCopy((Array)this.buffer, 0, (Array)numArray, 0, this.buffer.Length);
            Buffer.BlockCopy((Array)data, ofs, (Array)numArray, this.buffer.Length, count);
            this.buffer = numArray;
        }

        public void WriteDouble(double d)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(d)));
        }

        public void WriteFloat(float f)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(f)));
        }

        public void WriteInt(int i)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(i)));
        }

        public void WriteLong(long l)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(l)));
        }

        public void WriteShort(short s)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(s)));
        }

        public void WriteUShort(ushort us)
        {
            this.WriteBytes(this.ReverseOrder(BitConverter.GetBytes(us)));
        }

        public void WriteUTF(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                this.WriteUShort((ushort)0);
            }
            else
            {
                int num1 = 0;
                for (int index = 0; index < str.Length; ++index)
                {
                    int num2 = (int)str[index];
                    if (num2 >= 1 && num2 <= (int)sbyte.MaxValue)
                        ++num1;
                    else if (num2 > 2047)
                        num1 += 3;
                    else
                        num1 += 2;
                }
                if (num1 > 32768)
                    throw new FormatException("String length cannot be greater then 32768 !");
                this.WriteUShort(Convert.ToUInt16(num1));
                this.WriteBytes(System.Text.Encoding.UTF8.GetBytes(str));
            }
        }
    }

}

