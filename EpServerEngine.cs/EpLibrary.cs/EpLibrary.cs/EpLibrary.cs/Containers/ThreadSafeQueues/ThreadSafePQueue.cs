/*! 
@file ThreadSafePQueue.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief ThreadSafePQueue Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2014 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

A ThreadSafePQueue Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace EpLibrary.cs
{
    
    /// A class for Thread Safe Priority Queue.
    
    /// <typeparam name="DataType">the element type</typeparam>
    public class ThreadSafePQueue<DataType> where DataType : IComparable<DataType>
    {
        
        /// Default constructor
        
        public ThreadSafePQueue()
        {
            
        }

        
        /// Default copy constructor
        
        /// <param name="b">object to copy from</param>
		public ThreadSafePQueue(ThreadSafePQueue<DataType> b)
        {
            m_queue = new List<DataType>(b.GetQueue());
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
        
        /// Insert the new item into the priority queue.
        
        /// <param name="data">The inserting data.</param>
        public void Enqueue(DataType data)
        {
            lock(m_queueLock)
            {
                m_queue.Add(data);
                m_queue.Sort();
            }
		    
        }
        
        /// Erase the given item from the queue.
        
        /// <param name="data">The data to erase.</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Erase(DataType data)
        {
            lock (m_queueLock)
            {
                if (m_queue.Contains(data))
                {
                    for (int idx = m_queue.Count - 1; idx >= 0; idx--)
                    {
                        if (m_queue[idx].Equals(data))
                        {
                            m_queue.RemoveAt(idx);
                            return true;
                        }
                    }
                }
                return false;

            }

        }

        
        /// Remove the first item from the queue.
        
        public virtual DataType Dequeue()
        {
            lock (m_queueLock)
            {
                DataType data = m_queue[0];
                m_queue.RemoveAt(0);
                return data;
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
        
        protected List<DataType> m_queue = new List<DataType>();

        
        /// lock
        
        protected Object m_queueLock = new Object();

    }
}
