/*! 
@file ThreadEx.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief ThreadEx Interface
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

A ThreadEx Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace EpLibrary.cs
{
    
    /// Enumerator for Thread Operation Code
    
    public enum ThreadOpCode
    {
        
        /// The thread is started when it is created.
        
        CREATE_START = 0,
        
        /// The thread is suspended when it is created.
        
        CREATE_SUSPEND
    };

    
    /// Enumerator for Thread Status
    
    public enum ThreadStatus
    {
        
        /// The thread is started and running.
        
        STARTED = 0,
        
        /// The thread is suspended.
        
        SUSPENDED,
        
        /// The thread is terminated.
        
        TERMINATED
    };

    
    /// Enumerator for Thread Terminate Result
    
    public enum TerminateResult
    {
        
        /// Failed to terminate the thread 
        
        FAILED = 0,
        
        /// The thread terminated gracefully
        
        GRACEFULLY_TERMINATED,
        
        /// The thread terminated forcefully
        
        FORCEFULLY_TERMINATE,
        
        /// The thread was not running
        
        NOT_ON_RUNNING,
    };
    
    /// A class that implements base thread class operation.
    
    public class ThreadEx
    {
        
        /// thread handle
        
        private Thread m_threadHandle;
        		
        
        /// ThreadPriority
        
		private ThreadPriority m_threadPriority;
		
        
        /// Parent Thread Handle
        
		private Thread m_parentThreadHandle;
		
        
        /// Thread Status
        
		private ThreadStatus m_status;

        
        /// thread Func
        
        private Action m_threadFunc;

        
        /// thread parameterized Func
        
        private Action<object> m_threadParameterizedFunc;
        
        /// parameter object for parameterized function
        
        private object m_parameter;
        
        /// Lock
        
        private Object m_threadLock = new Object();
        
        /// exit code
        
		private ulong m_exitCode;

        

        
        /// Default Constructor
        
        /// <param name="priority">The priority of the thread.</param>
 		public ThreadEx(ThreadPriority priority=ThreadPriority.Normal)
        {
            m_threadHandle=null;
            m_threadPriority=priority;
            m_parentThreadHandle=null;
            m_status=ThreadStatus.TERMINATED;
            m_exitCode=0;
            m_threadFunc = null;
            m_threadParameterizedFunc = null;
            m_parameter = null;
        }

        
        /// Default Constructor
        
        /// <param name="threadFunc">the function for the thread</param>
        /// <param name="priority">The priority of the thread.</param>
		public ThreadEx(Action threadFunc, ThreadPriority priority=ThreadPriority.Normal)
        {
            m_threadHandle = null;
            m_threadPriority = priority;
            m_parentThreadHandle = null;
            m_status = ThreadStatus.TERMINATED;
            m_exitCode = 0;
            m_threadFunc = threadFunc;
            m_threadParameterizedFunc = null;
            m_parameter = null;

	        m_parentThreadHandle=Thread.CurrentThread;
	        m_threadHandle=new Thread(ThreadEx.entryPoint);
            m_threadHandle.Priority=m_threadPriority;
            m_threadHandle.Start(this);
            m_status=ThreadStatus.STARTED; 
                  

        }

        
        /// Default Constructor
        
        /// <param name="threadParameterizedFunc">the parameterized function for the thread</param>
        /// <param name="priority">The priority of the thread.</param>
        public ThreadEx(Action<object> threadParameterizedFunc,object parameter, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_threadHandle = null;
            m_threadPriority = priority;
            m_parentThreadHandle = null;
            m_status = ThreadStatus.TERMINATED;
            m_exitCode = 0;
            m_threadFunc = null;
            m_threadParameterizedFunc = threadParameterizedFunc;
            m_parameter = parameter;

            m_parentThreadHandle = Thread.CurrentThread;
            m_threadHandle = new Thread(ThreadEx.entryPoint);
            m_threadHandle.Priority = m_threadPriority;
            m_threadHandle.Start(this);
            m_status = ThreadStatus.STARTED;


        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
        public ThreadEx(ThreadEx b)
        {
            m_threadFunc=b.m_threadFunc;
            m_threadParameterizedFunc = b.m_threadParameterizedFunc;
            m_parameter = b.m_parameter;
	        if(m_threadFunc!=null||m_parentThreadHandle!=null)
	        {
		        m_parentThreadHandle=b.m_parentThreadHandle;
		        m_threadHandle=b.m_threadHandle;
		        m_threadPriority=b.m_threadPriority;
		        m_status=b.m_status;
		        m_exitCode=b.m_exitCode;

		        b.m_parentThreadHandle=null;
		        b.m_threadHandle=null;
		        b.m_status=ThreadStatus.TERMINATED;
		        b.m_exitCode=0;
            }
	        else
	        {
		        m_threadHandle=null;
		        m_threadPriority=b.m_threadPriority;
		        m_parentThreadHandle=null;
		        m_exitCode=0;

                m_status = ThreadStatus.TERMINATED;
	        }
        }

        ~ThreadEx()
        {
            resetThread();
        }
		
        
        /// Start the Thread according to parameters given.
        
        /// <param name="opCode">The operation code for creating thread.</param>
        /// <param name="stackSize">The stack size for the thread.</param>
        /// <returns>true, if succeeded, otherwise false.</returns>
		public bool Start(ThreadOpCode opCode=ThreadOpCode.CREATE_START, int stackSize=0)
        {
            lock(m_threadLock)
            {
                m_parentThreadHandle=Thread.CurrentThread;
                if(m_status==ThreadStatus.TERMINATED&& m_threadHandle==null)
                {
                    m_threadHandle=new Thread(ThreadEx.entryPoint,stackSize);
                    if (m_threadHandle != null)
                    {
                        m_threadHandle.Priority = m_threadPriority;
                        if (opCode == ThreadOpCode.CREATE_START)
                        {
                            m_threadHandle.Start(this);
                            m_status = ThreadStatus.STARTED;
                        }
                        else
                            m_status = ThreadStatus.SUSPENDED;
                        return true;
                    }

                }
                //	System::OutputDebugString(_T("The thread (%x): Thread already exists!\r\n"),m_threadId);
	                return false;
            }
        }

        
        /// Resume the suspended thread.
        
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Resume()
        {
            lock (m_threadLock)
            {
                if (m_status == ThreadStatus.SUSPENDED && m_threadHandle != null)
                {
                    m_threadHandle.Resume();
                    m_status = ThreadStatus.STARTED;
                    return true;
                }
            }
            //	System::OutputDebugString(_T("The thread (%x): Thread must be in suspended state in order to resume!\r\n"),m_threadId);
            return false;
        }

        
        /// Suspend the running thread.
        
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Suspend()
        {

            if(m_status==ThreadStatus.STARTED && m_threadHandle!=null)
            {
                lock(m_threadLock)
                {
                    m_status=ThreadStatus.SUSPENDED;
                }
                m_threadHandle.Suspend();
                return true;
            }
            //	System::OutputDebugString(_T("The thread (%x): Thread must be in running state in order to suspend!\r\n"),m_threadId);
            return false;
            
        }

        
        /// Terminate the running or suspended thread.
        
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Terminate()
        {
            Debug.Assert(m_threadHandle != Thread.CurrentThread, "Exception : Thread should not terminate self.");

            if (m_status != ThreadStatus.TERMINATED && m_threadHandle != null)
            {
                lock (m_threadLock)
                {
                    m_status = ThreadStatus.TERMINATED;
                    m_exitCode = 1;
                    m_threadHandle.Abort();
                    m_threadHandle = null;
                    m_parentThreadHandle = null;
                }
                ulong exitCode = m_exitCode;
                onTerminated(exitCode);
                return true;
            }
            return true;
        }

        
        /// Wait for thread to terminate
        
        /// <param name="tMilliseconds">the time-out interval, in milliseconds.</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool WaitFor(int tMilliseconds = Timeout.Infinite)
        {
            if(m_status!=ThreadStatus.TERMINATED && m_threadHandle!=null)
	        {
		        return m_threadHandle.Join(tMilliseconds);
	        }
	        else
	        {
                //	System::OutputDebugString(_T("The thread (%x): Thread is not started!\r\n"),m_threadId);
		        return false;
	        }
        }

        
        /// Join the thread
        
        public void Join()
        {
            if (m_status != ThreadStatus.TERMINATED && m_threadHandle != null)
            {
                m_threadHandle.Join();
            }
        }

        
        /// Check if the thread class is joinable
        
        /// <returns>true if joinable otherwise false</returns>
        public bool Joinable()
        {
            return (m_status != ThreadStatus.TERMINATED && m_threadHandle != null);
        }

        
        /// Detach the thread
        
        public void Detach()
        {
            Debug.Assert(Joinable() == true);
            lock (m_threadLock)
            {
                m_status = ThreadStatus.TERMINATED;
                m_threadHandle = null;
                m_parentThreadHandle = null;
                m_exitCode = 0;
            }
        }

        
        /// Wait for thread to terminate, and if not terminated, then Terminate.
        
        /// <param name="tMilliseconds">the time-out interval, in milliseconds.</param>
        /// <returns>the terminate result of the thread</returns>
        public TerminateResult TerminateAfter(int tMilliseconds)
        {
           	if(m_status!=ThreadStatus.TERMINATED && m_threadHandle!=null)
	        {
		        bool status=m_threadHandle.Join(tMilliseconds);
                if(status)
                {
                    return TerminateResult.GRACEFULLY_TERMINATED;
                }
                else
                {
                    if (Terminate())
                        return TerminateResult.FORCEFULLY_TERMINATE;
                    return TerminateResult.FAILED;
                }
	        }
	        else
	        {
		        //System::OutputDebugString(_T("The thread (%x): Thread is not started!\r\n"),m_threadId);
		        return TerminateResult.NOT_ON_RUNNING;
	        }
        }

        
        /// Return the parent's Thread Handle.
        
        /// <returns>the parent's Thread Handle.</returns>
		public Thread GetParentThreadHandle()
		{
			return m_parentThreadHandle;
		}

        
        /// Return the Thread Status.
        
        /// <returns>the current thread status</returns>
		public ThreadStatus GetStatus()
		{
			return m_status;
		}

        
        /// Return the Thread Exit Code.
        
        /// <returns>the thread exit code.</returns>
        /// <remarks>0 means successful termination, 1 means unsafe termination.</remarks>
		public ulong GetExitCode()
		{
			return m_exitCode;
		}

        
        /// Return the current Thread Priority.
        
        /// <returns>the current Thread Priority.</returns>
        public ThreadPriority GetPriority()
        {
            return m_threadPriority;
        }

        
        /// Set Priority of the thread
        
        /// <param name="priority">The priority of the thread</param>
        /// <returns>true if successfully set otherwise false</returns>
        public bool SetPriority(ThreadPriority priority)
        {
            m_threadPriority = priority;
            m_threadHandle.Priority = priority;
            return true;
        }

        
        /// Return the Thread Handle.
        
        /// <returns>the current thread handle.</returns>
		protected Thread getHandle()
		{
			return m_threadHandle;
		}


        
        /// Actual Thread Code.
        
        /// <remarks>Subclass should override this function for executing the thread function.</remarks>
        protected virtual void execute()
        {
            if (m_threadFunc != null)
                m_threadFunc();
            else if (m_threadParameterizedFunc != null)
                m_threadParameterizedFunc(m_parameter);
        }

        
        /// Calls when the thread terminated.
        
        /// <param name="exitCode">the exit code of the thread</param>
        /// <param name="isInDeletion">the flag whether the thread class is in deletion or not</param>
        protected virtual void onTerminated(ulong exitCode, bool isInDeletion = false)
        {
        }

        
        /// Terminate the thread successfully.
        
        private void successTerminate()
        {
            lock (m_threadLock)
            {
                m_status = ThreadStatus.TERMINATED;
                m_threadHandle = null;
                m_parentThreadHandle = null;
                m_exitCode = 0;
            }
            
            onTerminated(m_exitCode);
        }

        
        /// Running the thread when thread is created.
        
        /// <returns>the exit code of the current thread.</returns>
        private int run()
        {
            execute();
            successTerminate();
            return 0;
        }
        
        /// Reset Thread
        
        private void resetThread()
        {
            if(m_status!=ThreadStatus.TERMINATED)
	        {
		        m_exitCode=1;
		        m_threadHandle.Abort();
		        onTerminated(m_exitCode,true);
	        }

	        m_threadHandle=null;
	        m_parentThreadHandle=null;
	        m_exitCode=0;
            m_status = ThreadStatus.TERMINATED;
        }

        
        /// Entry point for the thread
        
        /// <param name="pThis">The argument for the thread (this for current case)</param>
        private static void entryPoint(object pThis)
        {
            ThreadEx pt = (ThreadEx)pThis;
            pt.run();
        }


    }
}
