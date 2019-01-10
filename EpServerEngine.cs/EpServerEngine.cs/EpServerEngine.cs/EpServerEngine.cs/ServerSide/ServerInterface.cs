/*! 
@file ServerInterface.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief ServerInterface Interface
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

A ServerInterface Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace EpServerEngine.cs
{
    
    /// Server option class
    
    public sealed class ServerOps
    {
        
        /// callback object
        
        public INetworkServerCallback callBackObj;

        
        /// port
        
        public String port;

        
        /// Default constructor
        
        public ServerOps()
        {
            callBackObj = null;
            port = ServerConf.DEFAULT_PORT;
        }
        
        /// Default constructor
        
        /// <param name="callBackObj">callback object</param>
        /// <param name="port">port</param>
        public ServerOps(INetworkServerCallback callBackObj, String port)
        {
            this.port = port;
            this.callBackObj = callBackObj;
        }

        
        /// Default server option
        
        public static ServerOps defaultServerOps = new ServerOps();
    };

    
    /// Server interface
    
    public interface INetworkServer
    {
        
        /// Return the port
        
        /// <returns>port</returns>
        String GetPort();

        
        /// Start the server with given option
        
        /// <param name="ops">option for the server</param>
        void StartServer(ServerOps ops);

        
        /// Stop the server
        
        void StopServer();

        
        /// Check whether server is started or not
        
        /// <returns>true if server is started, otherwise false</returns>
        bool IsServerStarted();
        
        /// Shutdown all the client, connected
        
        void ShutdownAllClient();

        
        /// Broadcast the given packet to all the client, connected
        
        /// <param name="packet">packet to broadcast</param>
        void Broadcast(Packet packet);

        
        /// Return the connected client list
        
        /// <returns>the connected client list</returns>
        List<IocpTcpSocket> GetClientSocketList();

        
        /// Detach the given client from the server management
        
        /// <param name="clientSocket">the client to detach</param>
        /// <returns>true if successful, otherwise false</returns>
        bool DetachClient(IocpTcpSocket clientSocket);

    }

    
    /// Server callback interface
    
    public interface INetworkServerCallback
    {
        
        /// Server started callback
        
        /// <param name="server">server</param>
        /// <param name="status">start status</param>
        void OnServerStarted(INetworkServer server, StartStatus status);
        
        /// Accept callback
        
        /// <param name="server">server</param>
        /// <param name="ipInfo">connection info</param>
        /// <returns>the socket callback interface</returns>
        INetworkSocketCallback OnAccept(INetworkServer server, IPInfo ipInfo);
        
        /// Server stopped callback
        
        /// <param name="server">server</param>
        void OnServerStopped(INetworkServer server);
    };

    
    /// Socket interface
    
    public interface INetworkSocket
    {
        
        /// Disconnect the client
        
        void Disconnect();

        
        /// Check if the connection is alive
        
        /// <returns>true if the connection is alive, otherwise false</returns>
        bool IsConnectionAlive();

        
        /// Send given packet to the client
        
        /// <param name="packet">the packet to send</param>
        void Send(Packet packet);

        
        /// Return the IP information of the client
        
        /// <returns>the IP information of the client</returns>
        IPInfo GetIPInfo();

        
        /// Return the server managing this socket
        
        /// <returns>the server managing this socket</returns>
        INetworkServer GetServer();

    }

    
    /// Socket callback interface
    
    public interface INetworkSocketCallback
    {
        
        /// NewConnection callback
        
        /// <param name="socket">client socket</param>
        void OnNewConnection(INetworkSocket socket);

        
        /// Receive callback
        
        /// <param name="socket">client socket</param>
        /// <param name="receivedPacket">received packet</param>
        void OnReceived(INetworkSocket socket, Packet receivedPacket);

        
        /// Send callback
        
        /// <param name="socket">client socket</param>
        /// <param name="status">stend status</param>
        void OnSent(INetworkSocket socket, SendStatus status);

        
        /// Disconnect callback
        
        /// <param name="socket">client socket</param>
        void OnDisconnect(INetworkSocket socket);
    };
    
    
    /// IP End-point type
    
    public enum IPEndPointType
    {
        
        /// local
        
        LOCAL = 0,
        
        /// remote
        
        REMOTE
    }

    
    /// IP Information class
    
    public sealed class IPInfo
    {
        
        /// IP Address string
        
        String m_ipAddress;
        
        /// IP End-Point
        
        IPEndPoint m_ipEndPoint;
        
        /// IP End-Point type
        
        IPEndPointType m_ipEndPointType;

        
        /// Default constructor
        
        /// <param name="ipAddress">IP Address string</param>
        /// <param name="ipEndPoint">IP End-Point</param>
        /// <param name="ipEndPointType">IP End-Point type</param>
        public IPInfo(String ipAddress, IPEndPoint ipEndPoint, IPEndPointType ipEndPointType)
        {
            m_ipAddress = ipAddress;
            m_ipEndPoint = ipEndPoint;
            m_ipEndPointType = ipEndPointType;
        }
        
        /// Return the IP address string
        
        /// <returns>the IP address string</returns>
        public String GetIPAddress()
        {
            return m_ipAddress;
        }

        
        /// Return the IP End-point
        
        /// <returns>the IP End-point</returns>
        public IPEndPoint GetIPEndPoint()
        {
            return m_ipEndPoint;
        }

        
        /// Return the IP End-point type
        
        /// <returns>the IP End-point type</returns>
        public IPEndPointType GetIPEndPointType()
        {
            return m_ipEndPointType;
        }
    }
}
