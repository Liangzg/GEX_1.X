using System;
using System.Collections;

namespace NetCore
{
    public class ByteArrayQueue
    {
        protected ArrayList m_queue;
        public ByteArrayQueue()
        {
            m_queue = new ArrayList();
        }

        public void pushBack(ByteArray buffer)
        {
            m_queue.Add(buffer);
        }

        public void pushFront(ByteArray buffer)
        {
            m_queue.Insert(0, buffer);
        }

        public void popBack()
        {
            m_queue.RemoveAt(m_queue.Count);
        }
        public void popFront()
        {
            m_queue.RemoveAt(0);
        }
        //public void popBackAndDelete();
        //public void popFrontAndDelete();

        public void removeAll()
        {
            m_queue.Clear();
        }
        public void deleteAll()
        {
            m_queue.Clear();
        }

        public ByteArray front()
        {
            return (ByteArray)m_queue[0];
        }
        public ByteArray back()
        {
            return (ByteArray)m_queue[m_queue.Count];
        }
        public ByteArray frontAndPop()
        {
            if (m_queue.Count < 1)
            {
                return null;
            }
            ByteArray buf = (ByteArray)m_queue[0];
            m_queue.RemoveAt(0);
            return buf;
        }
        public ByteArray backAndPop()
        {
            if (m_queue.Count < 1)
            {
                return null;
            }
            ByteArray buf = (ByteArray)m_queue[m_queue.Count];
            m_queue.RemoveAt(m_queue.Count);
            return buf;
        }

        //public void lockQueue();
        //public void unlockQueue();

        public bool empty()
        {
            return m_queue.Count == 0;

        }
        
    }
}
