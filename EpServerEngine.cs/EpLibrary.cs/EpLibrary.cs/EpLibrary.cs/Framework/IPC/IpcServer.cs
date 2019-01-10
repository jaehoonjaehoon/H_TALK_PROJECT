﻿/*! 
@file IpcServer.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief IpcServer Interface
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

A IpcServer Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EpLibrary.cs;
namespace EpLibrary.cs
{
    
    /// A class for Interprocess Communication Server.
    
    public sealed class IpcServer : ThreadEx,IpcServerInterface, IpcPipeCallbackInterface
    {
        
        /// pipe list
        
        private List<IpcInterface> m_pipes=new List<IpcInterface>();

        
        /// flag whether the server is started
        
        private bool m_started=false;

        
        /// IPC server options
        
        private IpcServerOps m_options;

        
        /// Default Constructor
        
		public IpcServer()
        {
        }

        
        /// Default Destructor
        
		~IpcServer()
        {
            StopServer();
        }

        
        /// Get the pipe name of server
        
        /// <returns>the pipe name in string</returns>
		public string GetFullPipeName()
        {
            return m_options.m_pipeName;
        }

        
        /// Get the Maximum Instances of server
        
        /// <returns>the Maximum Instances</returns>
		public int GetMaximumInstances()
        {
            return m_options.m_maximumInstances;
        }

        
        /// Start the server
        
        /// <param name="ops">the server options</param>
		public  void StartServer(IpcServerOps ops)
        {
            if (ops.m_callBackObj == null)
            {
                throw new ArgumentNullException("callback cannot be null.");
            }
            m_options = ops;
            if (ops.m_numOfWriteBytes <= 0)
                m_options.m_numOfWriteBytes = IpcConf.DEFAULT_WRITE_BUF_SIZE;
            if (ops.m_numOfReadBytes <= 0)
                m_options.m_numOfReadBytes = IpcConf.DEFAULT_READ_BUF_SIZE;
            if (ops.m_maximumInstances <= 0)
                m_options.m_maximumInstances = IpcConf.DEFAULT_PIPE_INSTANCES;
            Start();
        }

        
        /// Actual server start function
        
        protected override void execute()
        {
            IpcStartStatus status = IpcStartStatus.SUCCESS;
            try
            {
                IpcPipeOps pipeOptions = new IpcPipeOps(m_options.m_pipeName,this, m_options.m_numOfReadBytes, m_options.m_numOfWriteBytes);
                for (int trav = 0; trav < m_options.m_maximumInstances; trav++)
                {
                    IpcPipe pipeInst = new IpcPipe(pipeOptions);
                    pipeInst.Create();
                }
                m_started = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                m_started = false;
                status = IpcStartStatus.FAIL_PIPE_CREATE_FAILED;
            }
            m_options.m_callBackObj.OnServerStarted(this, status);
            
            
        }

        
        /// Stop the server
        
        public void StopServer()
        {
            m_started = false;
            ShutdownAllClient();
        }

        
        /// Check if the server is started
        
        /// <returns>true if the server is started otherwise false</returns>
		public  bool IsServerStarted()
        {
            return m_started;
        }

        
        /// Terminate all clients' socket connected.
        
		public void ShutdownAllClient()
        {
            lock (m_pipes)
            {
                for(int trav=0;trav<m_pipes.Count;trav++)
	            {
                    m_pipes[trav].KillConnection();
	            }
                m_pipes.Clear();
            }
        }

        
        /// Get the maximum write data byte size
        
        /// <returns>the maximum write data byte size</returns>
		public int GetMaxWriteDataByteSize()
        {
            return m_options.m_numOfReadBytes;
        }

        
        /// Get the maximum read data byte size
        
        /// <returns>the maximum read data byte size</returns>
		public  int GetMaxReadDataByteSize()
        {
            return m_options.m_numOfWriteBytes;
        }

        
        ///  When accepted client tries to make connection.
        
        /// <param name="pipe">the pipe</param>
        /// <param name="status">status of connect</param>
        /// <remarks>hen this function calls, it is right before making connection,
        /// so user can configure the pipe before the connection is actually made.	</remarks>
        public void OnNewConnection(IpcInterface pipe, IpcConnectStatus status)
        {
            if (status == IpcConnectStatus.SUCCESS)
            {
                if (!m_pipes.Contains(pipe))
                    m_pipes.Add(pipe);
            }
            m_options.m_callBackObj.OnNewConnection(this, pipe, status);
        }

        
        /// Received the data from the client.
        
        /// <param name="pipe">the pipe which received the packet</param>
        /// <param name="receivedData">the received data</param>
        /// <param name="receivedDataByteSize">the received data byte size</param>
        public void OnReadComplete(IpcInterface pipe, byte[] receivedData, int receivedDataByteSize)
        {
            m_options.m_callBackObj.OnReadComplete(this, pipe, receivedData, receivedDataByteSize);
        }

        
        /// Received the packet from the client.
        
        /// <param name="pipe">the pipe which wrote the packet</param>
        /// <param name="status">the status of write</param>
        public void OnWriteComplete(IpcInterface pipe, IpcWriteStatus status)
        {
            m_options.m_callBackObj.OnWriteComplete(this, pipe, status);
        }

        
        ///  The pipe is disconnected.
        
        /// <param name="pipe">the pipe, disconnected.</param>
        public void OnDisconnected(IpcInterface pipe)
        {
            lock (m_pipes)
            {
                if (m_pipes.Contains(pipe))
                    m_pipes.Remove(pipe);
                if(m_started)
                    pipe.Reconnect();
            }
            m_options.m_callBackObj.OnDisconnected(this, pipe);
        }

        
        /// Return the number of pipe connected
        
        /// <returns>number of pipe connected</returns>
        public int GetPipeCount()
        {
            lock (m_pipes)
            {
                return m_pipes.Count;
            }
        }
	
    }
}
