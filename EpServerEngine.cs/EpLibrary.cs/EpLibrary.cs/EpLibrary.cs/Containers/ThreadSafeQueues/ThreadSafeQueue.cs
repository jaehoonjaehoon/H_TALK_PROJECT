using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EpLibrary.cs
{
    
    /// A class for Thread Safe Queue.
    
    /// <typeparam name="DataType">the element type</typeparam>
    public class ThreadSafeQueue<DataType>
    {
        
        /// Default constructor
        
        public ThreadSafeQueue()
        {

        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
        public ThreadSafeQueue(ThreadSafeQueue<DataType> b)
        {
            m_queue = new Queue<DataType>(b.GetQueue());
        }


        
        /// Check if the queue is empty.
        
        /// <returns>true if the queue is empty, otherwise false.</returns>
        public bool IsEmpty()
        {
            lock (m_queueLock)
            {
                return m_queue.Count == 0;
            }
        }

        
        /// Check if the given obj exists in the queue.
        
        /// <param name="data">obj to check</param>
        /// <returns>true if exists, otherwise false.</returns>
        public bool Contains(DataType data)
        {
            lock (m_queueLock)
            {
                return m_queue.Contains(data);
            }
        }

        
        /// Return the size of the queue.
        
        public int Count
        {
            get
            {
                lock (m_queueLock)
                {
                    return m_queue.Count;
                }
            }
        }

        
        /// Return peek element
        
        /// <returns>the peek element of the queue </returns>
        public DataType Peek()
        {
            return Front();
        }

        
        /// Return the first item within the queue.
        
        /// <returns>the first element of the queue.</returns>
        public DataType Front()
        {
            lock (m_queueLock)
            {
                return m_queue.First();
            }
        }

        
        /// Return the last item within the queue.
        
        /// <returns>the last element of the queue.</returns>
        public DataType Back()
        {
            lock (m_queueLock)
            {
                return m_queue.Last();
            }
        }

        
        /// Insert the new item into the queue.
        
        /// <param name="data">The inserting data.</param>
        public virtual void Enqueue(DataType data)
        {
            lock (m_queueLock)
            {
                m_queue.Enqueue(data);
            }
        }


        
        /// Remove the first item from the queue.
        
        public virtual DataType Dequeue()
        {
            lock (m_queueLock)
            {
                return m_queue.Dequeue();
            }
        }

        
        /// Clear the queue.
        
        public void Clear()
        {
            lock (m_queueLock)
            {
                m_queue.Clear();
            }
        }

        
        /// Return the actual queue structure
        
        /// <returns>the actual queue structure</returns>
        public List<DataType> GetQueue()
        {
            lock (m_queueLock)
            {
                return new List<DataType>(m_queue);
            }
        }

        
        /// Actual queue structure
        
        protected Queue<DataType> m_queue = new Queue<DataType>();

        
        /// lock
        
        protected Object m_queueLock = new Object();

    }
}
