/*! 
@file IocpTcpServer.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief IocpTcpServer Interface
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

A IocpTcpServer Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using EpLibrary.cs;

namespace EpServerEngine.cs
{
    
    /// IOCP TCP Server
    
    public sealed class IocpTcpServer:ThreadEx, INetworkServer
    {
        
        /// port
        
        private String m_port=ServerConf.DEFAULT_PORT;
        
        /// listner
        
        private TcpListener m_listener=null;
        
        /// server option
        
        private ServerOps m_serverOps = null;

        
        /// callback object
        
        private INetworkServerCallback m_callBackObj=null;
        
        /// general lock
        
        private Object m_generalLock = new Object();

        
        /// client socket list lock
        
        private Object m_listLock = new Object();
        
        /// client socket list
        
        private List<IocpTcpSocket> m_socketList=new List<IocpTcpSocket>();

        
        /// Default constructor
        
        public IocpTcpServer()
            : base()
        {
        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
        public IocpTcpServer(IocpTcpServer b)
            : base(b)
        {
            m_port = b.m_port;
            m_serverOps = b.m_serverOps;

        }

        ~IocpTcpServer()
        {
            if(IsServerStarted())
                StopServer();
        }

        
        /// Return port
        
        /// <returns>port</returns>
        public String GetPort()
        {
            return m_port;
        }

        
        /// Callback Exception class
        
        private class CallbackException : Exception
        {
            
            /// Default constructor
            
            public CallbackException()
                : base()
            {

            }

            
            /// Default constructor
            
            /// <param name="message">message for exception</param>
            public CallbackException(String message)
                : base(message)
            {

            }
        }

        
        /// Start the server and start accepting the client
        
        protected override void execute()
        {
            StartStatus status=StartStatus.FAIL_SOCKET_ERROR;
            try
            {
                lock (m_generalLock)
                {
                    if (IsServerStarted())
                    {
                        status = StartStatus.FAIL_ALREADY_STARTED;
                        throw new CallbackException();
                    }

                    m_callBackObj = m_serverOps.callBackObj;
                    m_port = m_serverOps.port;

                    if (m_port == null || m_port.Length == 0)
                    {
                        m_port = ServerConf.DEFAULT_PORT;
                    }


                    m_listener = new TcpListener(IPAddress.Any, Convert.ToInt32(m_port));
                    m_listener.Start();
                    m_listener.BeginAcceptTcpClient(new AsyncCallback(IocpTcpServer.onAccept), this);
                }
            }
            catch (CallbackException)
            {
                m_callBackObj.OnServerStarted(this, status);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (m_listener != null)
                    m_listener.Stop();
                m_listener = null;
                m_callBackObj.OnServerStarted(this, StartStatus.FAIL_SOCKET_ERROR);
                return;
            }
            m_callBackObj.OnServerStarted(this, StartStatus.SUCCESS);
        }

        
        /// Accept callback function
        
        /// <param name="result">result</param>
        private static void onAccept(IAsyncResult result)
        {
            IocpTcpServer server = result.AsyncState as IocpTcpServer;
            TcpClient client=null;
            try { client = server.m_listener.EndAcceptTcpClient(result); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (client != null)
                    client.Close();
                server.StopServer();
                return; 
            }
            
            try { server.m_listener.BeginAcceptTcpClient(new AsyncCallback(IocpTcpServer.onAccept), server); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace); 
                if (client != null)
                    client.Close();
                server.StopServer(); 
                return; 
            }

            IocpTcpSocket socket = new IocpTcpSocket(client, server);
            INetworkSocketCallback socketCallbackObj=server.m_callBackObj.OnAccept(server, socket.GetIPInfo());
            if (socketCallbackObj == null)
            {
                socket.Disconnect();
            }
            else
            {
                socket.SetSocketCallback(socketCallbackObj);
                socket.Start();
                lock (server.m_listLock)
                {
                    server.m_socketList.Add(socket);
                }
            }

        }

        
        /// Start the server with given option
        
        /// <param name="ops">options</param>
        public void StartServer(ServerOps ops)
        {
            if (ops == null)
                ops = ServerOps.defaultServerOps;
            if (ops.callBackObj == null)
                throw new NullReferenceException("callBackObj is null!");
            lock (m_generalLock)
            {
                m_serverOps = ops;
            }
            Start();
        }
        
        /// Stop the server
        
        public void StopServer()
        {
            lock (m_generalLock)
            {
                if (!IsServerStarted())
                    return;
                m_listener.Stop();
                m_listener = null;
            }
            ShutdownAllClient();

            if(m_callBackObj!=null) 
                m_callBackObj.OnServerStopped(this);
        }

        
        /// Check if the server is started
        
        /// <returns>true if the server is started, otherwise false</returns>
        public bool IsServerStarted()
        {
            if (m_listener != null)
                return true;
            return false;
        }
        
        /// Shut down all the client, connected
        
        public void ShutdownAllClient()
        {
            lock (m_listLock)
            {
                for (int trav = m_socketList.Count - 1; trav >= 0; trav--)
                {
                    m_socketList[trav].Disconnect();
                }
                m_socketList.Clear();
            }
        }
        
        /// Broadcast the given packet to the all client, connected
        
        /// <param name="packet">the packet to broadcast</param>
        public void Broadcast(Packet packet)
        {
            List<IocpTcpSocket> socketList = GetClientSocketList();

            foreach (IocpTcpSocket socket in socketList)
            {
                socket.Send(packet);
            }
            m_socketList.Clear();
            
        }

        
        /// Return the client socket list
        
        /// <returns>the client socket list</returns>
        public List<IocpTcpSocket> GetClientSocketList()
        {
            lock (m_listLock)
            {
                return new List<IocpTcpSocket>(m_socketList);
            }
        }

        
        /// Detach the given client from the server management
        
        /// <param name="clientSocket">the client to detach</param>
        /// <returns></returns>
        public bool DetachClient(IocpTcpSocket clientSocket)
        {
            lock (m_listLock)
            {
                return m_socketList.Remove(clientSocket);
            }
        }

    }
}
