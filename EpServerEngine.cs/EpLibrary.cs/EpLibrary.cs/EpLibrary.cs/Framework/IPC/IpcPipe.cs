/*! 
@file IpcPipe.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief IpcPipe Interface
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

A IpcPipe Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;

namespace EpLibrary.cs
{

    
    /// IPC Pipe Options
    
    public sealed class IpcPipeOps
    {
        
        /// Callback Object
        
        public IpcPipeCallbackInterface m_callBackObj;
        
        /// Name of the pipe
        
        public string m_pipeName;

        
        /// read byte size
        
        public int m_numOfReadBytes;
        
        /// write byte size
        
        public int m_numOfWriteBytes;

        
        /// Default Constructor
        
        /// <param name="pipeName">the name of the pipe</param>
        /// <param name="numOfReadyBytes">maximum read buffer size</param>
        /// <param name="numOfWriteBytes">maximum write buffer size</param>
        /// <param name="callBackObj">callback object</param>
        public IpcPipeOps(string pipeName, IpcPipeCallbackInterface callBackObj, int numOfReadyBytes = IpcConf.DEFAULT_READ_BUF_SIZE, int numOfWriteBytes = IpcConf.DEFAULT_WRITE_BUF_SIZE)
        {
            m_pipeName = pipeName;
            m_callBackObj = callBackObj;
            m_numOfReadBytes = numOfReadyBytes;
            m_numOfWriteBytes = numOfWriteBytes;
        }

        
        /// Default Constructor
        
        public IpcPipeOps()
        {
            m_callBackObj = null;
            m_pipeName = null;
            m_numOfReadBytes = IpcConf.DEFAULT_READ_BUF_SIZE;
            m_numOfWriteBytes = IpcConf.DEFAULT_WRITE_BUF_SIZE;

        }

        
        /// Default IPC Pipe options
        
        public static IpcPipeOps defaultIpcPIpeOps;
    };

    
    /// Pipe Callback Interface
    
    public interface IpcPipeCallbackInterface
    {
        
        ///  When accepted client tries to make connection.
        
        /// <param name="pipe">the pipe</param>
        /// <param name="status">status of connect</param>
        /// <remarks>when this function calls, it is right before making connection,
        /// so user can configure the pipe before the connection is actually made.	</remarks>
        void OnNewConnection(IpcInterface pipe, IpcConnectStatus status);

        
        /// Received the data from the client.
        
        /// <param name="pipe">the pipe which received the packet</param>
        /// <param name="receivedData">the received data</param>
        /// <param name="receivedDataByteSize">the received data byte size</param>
        void OnReadComplete(IpcInterface pipe, byte[] receivedData, int receivedDataByteSize);

        
        /// Received the packet from the client.
        
        /// <param name="pipe">the pipe which wrote the packet</param>
        /// <param name="status">the status of write</param>
        void OnWriteComplete(IpcInterface pipe, IpcWriteStatus status);

        
        ///  The pipe is disconnected.
        
        /// <param name="pipe">the pipe, disconnected.</param>
        void OnDisconnected(IpcInterface pipe);
    };

    
    /// IPC Pipe class
    
    public sealed class IpcPipe:IpcInterface
    {

        
        /// Pipe handle
        
        private NamedPipeServerStream m_pipeHandle;
        
        /// flag whether the pipe is connected
        
        private bool m_connected;
        
        /// Pipe options
        
        private IpcPipeOps m_options;

        
        /// Pipe security
        
        private readonly PipeSecurity m_ps;

        
        /// Write buffer queue
        
        Queue<PipeWriteElem> m_writeQueue=new Queue<PipeWriteElem>();
        
        /// Read buffer
        
        byte[] m_readBuffer;

        
        /// General lock object
        
        Object m_generalLock = new Object();

        
        /// Default Constructor
        
        /// <param name="options">the pipe options</param>
        public IpcPipe(IpcPipeOps options)
        {
            if (options == null)
                options = IpcPipeOps.defaultIpcPIpeOps;
            if (options.m_callBackObj != null)
                throw new ArgumentException("callBackObj is null!");
            lock (m_generalLock)
            {
                m_options = options;
                if (options.m_numOfWriteBytes == 0)
                    m_options.m_numOfWriteBytes = IpcConf.DEFAULT_WRITE_BUF_SIZE;
                if (options.m_numOfReadBytes == 0)
                    m_options.m_numOfReadBytes = IpcConf.DEFAULT_READ_BUF_SIZE;

            }
            m_readBuffer = new byte[m_options.m_numOfReadBytes];
             // Provide full access to the current user so more pipe instances can be created
            m_ps = new PipeSecurity();
            m_ps.AddAccessRule(
                new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow)
            );
            m_ps.AddAccessRule(
                new PipeAccessRule(
                    new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow
                )
            );
        }

        
        /// Default Destructor
        
		~IpcPipe()
        {
            KillConnection();
        }

        
        /// Create the pipe
        
        /// <returns> true if successfully created otherwise false</returns>
		public bool Create()
        {
            try
            {
                m_pipeHandle = new NamedPipeServerStream(
                m_options.m_pipeName,
                PipeDirection.InOut,
               -1,     // maximum instances
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                m_options.m_numOfWriteBytes,
                m_options.m_numOfReadBytes,
                m_ps
                );
                m_pipeHandle.BeginWaitForConnection(OnClientConnected, this);
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                return false;
            }
             
        }

        
        /// Kill current connection and wait for other connection
        
        public void Reconnect()
        {
            KillConnection();
            Create();
        }

        
        /// Write data to the pipe
        
        /// <param name="data">the data to write</param>
        /// <param name="offset">offset to start write from given data</param>
        /// <param name="dataByteSize">byte size of the data to write</param>
		public void Write(byte[] data,int offset,int dataByteSize)
        {
            if (dataByteSize > m_options.m_numOfWriteBytes)
                throw new ArgumentException();
            PipeWriteElem elem = new PipeWriteElem(data,offset,dataByteSize);
            lock(m_writeQueue)
            {
                if(m_writeQueue.Count>0)
                {
                    m_writeQueue.Enqueue(elem);
                }
                else
                {
                    try
                    {
                        m_pipeHandle.BeginWrite(elem.m_data, 0, elem.m_dataSize, OnWriteComplete, this);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        if(IsConnectionAlive())
                            KillConnection();	
                    }
                }
            }

        }

        
        /// Check if the connection is alive
        
        /// <returns>true if the connection is alive otherwise false</returns>
		public bool IsConnectionAlive()
        {
            return m_connected;
        }

        
        /// Kill the connection
        
		public void KillConnection()
        {
            lock (m_generalLock)
            {
                if (!IsConnectionAlive())
                {
                    return;
                }
                m_pipeHandle.Close();
                m_pipeHandle = null;
                m_connected = false;
            }

            lock (m_writeQueue)
            {
                m_writeQueue.Clear();
            }
            Thread t = new Thread(delegate()
            {
                m_options.m_callBackObj.OnDisconnected(this);
            });
            t.Start();
        }

        
        /// Handle on client connected
        
        /// <param name="result">AsyncResult</param>
        private void OnClientConnected(IAsyncResult result)
        {
            IpcPipe pipeInst = (IpcPipe)result.AsyncState;
            // Complete the client connection
            NamedPipeServerStream pipe = (NamedPipeServerStream)result.AsyncState;
            try
            {
                pipe.EndWaitForConnection(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                pipeInst.m_options.m_callBackObj.OnNewConnection(this, IpcConnectStatus.FAIL_WAIT_FOR_CONNECTION_FAILED);
                return;
            }
            
            try
            {
                m_pipeHandle.BeginRead(m_readBuffer, 0, m_options.m_numOfReadBytes, OnReadComplete, this);
                m_connected = true;
                m_options.m_callBackObj.OnNewConnection(this, IpcConnectStatus.SUCCESS);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                KillConnection();
                m_options.m_callBackObj.OnNewConnection(this, IpcConnectStatus.FAIL_READ_FAILED);
            }
            
        }
		
        
        /// Handle when read is completed
        
        /// <param name="result">AsyncResult</param>
		private void OnReadComplete(IAsyncResult result)
        {
            IpcPipe pipeInst = (IpcPipe)result.AsyncState;
            int readByte = 0;
            byte[] readBuffer = null;
            try
            {
                readByte = pipeInst.m_pipeHandle.EndRead(result);
                readBuffer = m_readBuffer.ToArray();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                pipeInst.KillConnection();
                return;
            }
            try
            {
                pipeInst.m_pipeHandle.BeginRead(pipeInst.m_readBuffer, 0, pipeInst.m_options.m_numOfReadBytes, OnReadComplete, pipeInst);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                pipeInst.KillConnection();
            }

            pipeInst.m_options.m_callBackObj.OnReadComplete(pipeInst, readBuffer, readByte);
        
        }

        
        ///Handles when Write is completed
        
        /// <param name="result">AsyncResult</param>
		private void OnWriteComplete(IAsyncResult result)
        {
            IpcPipe pipeInst = (IpcPipe)result.AsyncState;

            try
            {
                pipeInst.m_pipeHandle.EndWrite(result);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                KillConnection();
                pipeInst.m_options.m_callBackObj.OnWriteComplete(pipeInst, IpcWriteStatus.FAIL_WRITE_FAILED);
                return;
            }

            lock (pipeInst.m_writeQueue)
            {
                if (pipeInst.m_writeQueue.Count > 0)
                {
                    PipeWriteElem elem = pipeInst.m_writeQueue.Dequeue();
                    if (pipeInst.m_writeQueue.Count() > 0)
                    {
                        PipeWriteElem nextElem = pipeInst.m_writeQueue.Dequeue();

                        try
                        {
                            m_pipeHandle.BeginWrite(nextElem.m_data, nextElem.m_offset, nextElem.m_dataSize, OnWriteComplete, this);
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                            pipeInst.m_options.m_callBackObj.OnWriteComplete(pipeInst, IpcWriteStatus.SUCCESS);
                            KillConnection();
                            pipeInst.m_options.m_callBackObj.OnWriteComplete(pipeInst, IpcWriteStatus.FAIL_WRITE_FAILED);
                            return;
                        }
                    }
                    pipeInst.m_options.m_callBackObj.OnWriteComplete(pipeInst, IpcWriteStatus.SUCCESS);
                }
            }
        }


    }
}
