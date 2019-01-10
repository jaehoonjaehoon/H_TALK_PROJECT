/*! 
@file BaseWorkerThread.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief BaseWorkerThread Interface
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

A BaseWorkerThread Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace EpLibrary.cs
{
    
    /// Enumerator for Thread Life Policy
    
    public enum ThreadLifePolicy
    {
        
        /// The thread is infinitely looping.
        
        INFINITE = 0,
        
        /// The thread suspends if work pool is empty.
        
        SUSPEND_AFTER_WORK,
        
        /// The thread terminates if work pool is empty.
        
        TERMINATE_AFTER_WORK,
    };

    
    /// A class that implements Base Worker Thread Class.
    
    public abstract class BaseWorkerThread:ThreadEx
    {
        
        /// Default Constructor
        
        /// <param name="policy">the life policy of this worker thread.</param>
        public BaseWorkerThread(ThreadLifePolicy policy):base(ThreadPriority.Normal)
        {
            m_lifePolicy=policy;
            m_callBackFunc = null;

        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
		public BaseWorkerThread(BaseWorkerThread b):base(b)
        {
            lock(b.m_callBackLock)
            {
                m_lifePolicy=b.m_lifePolicy;
                m_callBackFunc = b.m_callBackFunc;
	            m_jobProcessor=b.m_jobProcessor;
	            m_workPool=b.m_workPool;
            }
            
        }

        ~BaseWorkerThread()
        {
            while(!m_workPool.IsEmpty())
	        {
		        m_workPool.Front().JobReport(JobStatus.INCOMPLETE);
		        m_workPool.Dequeue();
	        }
        }

		
        /// Push in the new work to the work pool.
		
        /// <param name="work">the new work to put into the work pool.</param>
		public void Push(BaseJob  work)
        {
            m_workPool.Enqueue(work);
            if(m_lifePolicy==ThreadLifePolicy.SUSPEND_AFTER_WORK)
                Resume();
        }

		
        /// Pop a work from the work pool.
		
		public BaseJob Pop()
        {
            return m_workPool.Dequeue();
        }

        
        /// Get First Job in the Job Queue.
        
        /// <returns>first job</returns>
		public BaseJob Front()
        {
            return m_workPool.Front();
        }
        
        /// Erase the given work from the work pool.
        
        /// <param name="work">the work to erase from the work pool</param>
        /// <returns>true if successful, otherwise false.</returns>
		public bool Erase(BaseJob work)
        {
            return m_workPool.Erase(work);
        }

        
        /// Return the life policy of this worker thread.
        
        /// <returns>the life policy of this worker thread.</returns>
		public ThreadLifePolicy GetLifePolicy()
		{
			return m_lifePolicy;
		}

		
        /// Set call back class to call when work is done.
		
        /// <param name="callBackFunc">the call back function.</param>
		public virtual void SetCallBackClass(Action<BaseWorkerThread> callBackFunc)
        {
            lock(m_callBackLock)
            {
                m_callBackFunc = callBackFunc;
            }
        }

        
        /// Get job count in work pool.
        
        /// <returns>the job count in work pool.</returns>
        public int GetJobCount()
        {
            return m_workPool.Count;
        }

        
        /// Set new Job Processor.
        
        /// <param name="jobProcessor">set new Job Processor for this thread.</param>
        public void SetJobProcessor(BaseJobProcessor jobProcessor)
        {
            m_jobProcessor = jobProcessor;
        }

        
        /// Get Job Processor.
        
        /// <returns>the Job Processor for this thread.</returns>
        public BaseJobProcessor GetJobProcessor()
        {
            return m_jobProcessor;
        }

        
        /// Wait for worker thread to terminate, and if not terminated, then Terminate.
        
        /// <param name="waitTimeInMilliSec">the time-out interval, in milliseconds.</param>
        /// <returns>the terminate result of the thread</returns>
        public virtual TerminateResult TerminateWorker(int waitTimeInMilliSec = Timeout.Infinite)
        {
            return TerminateAfter(waitTimeInMilliSec);
        }

	
        
        /// Pure Worker Thread Code.
        
		protected new abstract void execute();

        
        /// Call the Call Back Class if callback class is assigned.
        
        protected void callCallBack()
        {
            lock (m_callBackLock)
            {
                if (m_callBackFunc != null)
                    m_callBackFunc(this);
                m_callBackFunc = null;
            }
        }

        
        /// the work list
        
        protected JobScheduleQueue m_workPool;
        
        /// the life policy of the thread
        
        protected ThreadLifePolicy m_lifePolicy;
        
        /// the call back class
        
        protected Action<BaseWorkerThread> m_callBackFunc;

        
        /// callback Lock
        
        protected Object m_callBackLock = new Object();
        
        /// Job Processor
        
        protected BaseJobProcessor m_jobProcessor;
    }
}
