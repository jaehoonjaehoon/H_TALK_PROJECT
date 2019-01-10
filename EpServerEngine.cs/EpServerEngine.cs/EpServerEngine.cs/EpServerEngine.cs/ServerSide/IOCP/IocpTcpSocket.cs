/*! 
@file IocpTcpSocket.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief IocpTcpSocket Interface
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

A IocpTcpSocket Class.

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
    
    /// IOCP TCP Socket class
    
    public sealed class IocpTcpSocket:ThreadEx,INetworkSocket
    {
        
        /// actual client
        
        private TcpClient m_client=null;
        
        /// managing server
        
        private INetworkServer m_server = null;
        
        /// IP information
        
        private IPInfo m_ipInfo;
        
        /// general lock
        
        private Object m_generalLock = new Object();
        
        /// send lock
        
        private Object m_sendLock = new Object();
        
        /// send queue lock
        
        private Object m_sendQueueLock = new Object();
        
        /// send queue
        
        private Queue<PacketTransporter> m_sendQueue = new Queue<PacketTransporter>();
        
        /// callback object
        
        private INetworkSocketCallback m_callBackObj=null;
        
        
        /// send event
        
        private EventEx m_sendEvent = new EventEx();

        
        /// receive size packet
        
        private Packet m_recvSizePacket = new Packet(null, 4);
        
        /// send size packet
        
        private Packet m_sendSizePacket = new Packet(null, 4, false);

        
        /// flag for connection check
        
        private bool m_isConnected = false;

        
        /// Default constructor
        
        /// <param name="client">client</param>
        /// <param name="server">managing server</param>
        public IocpTcpSocket(TcpClient client, INetworkServer server):base()
        {
            m_client=client;
            m_server = server;
            IPEndPoint remoteIpEndPoint = m_client.Client.RemoteEndPoint as IPEndPoint;
            IPEndPoint localIpEndPoint = m_client.Client.LocalEndPoint as IPEndPoint;
            if (remoteIpEndPoint != null)
            {
                String socketHostName = remoteIpEndPoint.Address.ToString();
                m_ipInfo = new IPInfo(socketHostName, remoteIpEndPoint, IPEndPointType.REMOTE);
            }
            else if (localIpEndPoint != null)
            {
                String socketHostName = localIpEndPoint.Address.ToString();
                m_ipInfo = new IPInfo(socketHostName, localIpEndPoint, IPEndPointType.LOCAL);
            }
            
        }

        ~IocpTcpSocket()
        {
            if (IsConnectionAlive())
                Disconnect();
        }

        
        /// Get IP information
        
        /// <returns>IP information</returns>
        public IPInfo GetIPInfo()
        {
            return m_ipInfo;
        }

        
        /// Get managing server
        
        /// <returns>managing server</returns>
        public INetworkServer GetServer()
        {
            return m_server;
        }

        
        /// Set socket callback interface
        
        /// <param name="callBackObj">callback object</param>
        public void SetSocketCallback(INetworkSocketCallback callBackObj)
        {
            m_callBackObj = callBackObj;
        }
        
        /// Return the socket callback object
        
        /// <returns>the socket callback object</returns>
        public INetworkSocketCallback GetSocketCallback()
        {
            return m_callBackObj;
        }
        
        /// Start the new connection, and inform the callback object, that the new connection is made
        
        protected override void execute()
        {
            lock (m_generalLock)
            {
                m_isConnected = true;
            }
            startReceive();
            if(m_callBackObj!=null) 
                m_callBackObj.OnNewConnection(this);
        }

        
        /// Disconnect the client socket
        
        public void Disconnect()
        {
            lock (m_generalLock)
            {
                if (!IsConnectionAlive())
                    return;
                m_client.Close();
                m_isConnected = false;
            }
            m_server.DetachClient(this);

            lock (m_sendQueueLock)
            {
                m_sendQueue.Clear();
            }
            if (m_callBackObj != null)
            {
                Thread t = new Thread(delegate()
                {
                    m_callBackObj.OnDisconnect(this);
                });
                t.Start();
            }
        }

        
        /// Check if the connection is alive
        
        /// <returns>true if connection is alive, otherwise false</returns>
        public bool IsConnectionAlive()
        {
            return m_isConnected;
// 	        try
// 	        {
// 	            return m_client.Connected;
// 	        }
// 	        catch (Exception ex)
// 	        {
// 	            Console.WriteLine(ex.Message + " >" + ex.StackTrace);
// 	            return false;
// 	        }
        }

        
        /// Send given packet to the client
        
        /// <param name="packet">the packet to send</param>
        public void Send(Packet packet)
        {
            if (!IsConnectionAlive())
            {
                if (m_callBackObj != null)
                {
                    Thread t = new Thread(delegate()
                    {
                        m_callBackObj.OnSent(this, SendStatus.FAIL_NOT_CONNECTED);
                    });
                    t.Start();
                }
                return;
            }
            if (packet.GetPacketByteSize() <= 0)
            {
                if (m_callBackObj != null)
                {
                    Thread t = new Thread(delegate()
                    {
                        m_callBackObj.OnSent(this, SendStatus.FAIL_INVALID_PACKET);
                    });
                    t.Start();
                }
                return;
            }

            lock (m_sendLock)
            {
                PacketTransporter transport = new PacketTransporter(PacketType.SIZE, m_sendSizePacket, 0, 4, this, packet);
                m_sendSizePacket.SetPacket(BitConverter.GetBytes(packet.GetPacketByteSize()), 4);
                if (m_sendEvent.TryLock())
                {
                    try { m_client.Client.BeginSend(m_sendSizePacket.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), transport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        Disconnect(); 
                        return;
                    }
                }
                else
                {
                    lock (m_sendQueueLock)
                    {
                        m_sendQueue.Enqueue(transport);
                    }
                }
            }
        }

        
        /// Enumerator for packet type
        
        private enum PacketType
        {
            
            /// Send type
            
            SIZE = 0,
            
            /// Receive type
            
            DATA
        }
        
        /// Packet Transporter class
        
        private class PacketTransporter
        {
            
            /// packet to transport
            
            public Packet m_packet;
            
            /// data packet for send
            
            public Packet m_dataPacket;
            
            /// offset
            
            public int m_offset;
            
            /// packet size in byte
            
            public int m_size;
            
            /// client socket
            
            public IocpTcpSocket m_iocpTcpClient;
            
            /// packet type
            
            public PacketType m_packetType;
            
            /// callback object
            
            public INetworkSocketCallback m_callBackObj;
            
            /// Default constructor
            
            /// <param name="packetType">packet type</param>
            /// <param name="packet">packet</param>
            /// <param name="offset">offset</param>
            /// <param name="size">size of packet in byte</param>
            /// <param name="iocpTcpClient">client socket</param>
            /// <param name="dataPacket">data packet for send</param>
            public PacketTransporter(PacketType packetType, Packet packet, int offset, int size, IocpTcpSocket iocpTcpClient, Packet dataPacket = null)
            {
                m_packetType = packetType;
                m_packet = packet;
                m_offset = offset;
                m_size = size;
                m_iocpTcpClient = iocpTcpClient;
                m_dataPacket = dataPacket;
                m_callBackObj = iocpTcpClient.m_callBackObj;
            }
        }
        
        /// Start to receive packet from the server
        
        private void startReceive()
        {
            PacketTransporter transport = new PacketTransporter(PacketType.SIZE,m_recvSizePacket, 0, 4, this);
            try { m_client.Client.BeginReceive(m_recvSizePacket.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), transport); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                Disconnect();
                return;
            }
            
        }

        
        /// Receive callback function
        
        /// <param name="result">result</param>
        private static void onReceived(IAsyncResult result)
        {
            PacketTransporter transport = result.AsyncState as PacketTransporter;
            Socket socket = transport.m_iocpTcpClient.m_client.Client;
            
            int readSize=0;
            try { readSize = socket.EndReceive(result); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace); 
                transport.m_iocpTcpClient.Disconnect(); 
                return;
            }
            if (readSize == 0)
            {
                transport.m_iocpTcpClient.Disconnect();
                return;
            }
            if (readSize < transport.m_size)
            {
                transport.m_offset = transport.m_offset + readSize;
                transport.m_size = transport.m_size - readSize;
                try { socket.BeginReceive(transport.m_packet.GetPacket(), transport.m_offset, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), transport); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                    transport.m_iocpTcpClient.Disconnect(); 
                    return;
                }
            }
            else
            {
                if (transport.m_packetType == PacketType.SIZE)
                {
                    int shouldReceive = BitConverter.ToInt32(transport.m_packet.GetPacket(), 0);
                    Packet recvPacket = new Packet(null, shouldReceive);
                    PacketTransporter dataTransport = new PacketTransporter(PacketType.DATA, recvPacket, 0, shouldReceive, transport.m_iocpTcpClient);
                    try { socket.BeginReceive(recvPacket.GetPacket(), 0, shouldReceive, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), dataTransport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace); 
                        transport.m_iocpTcpClient.Disconnect(); 
                        return;
                    }
                }
                else
                {
                    PacketTransporter sizeTransport = new PacketTransporter(PacketType.SIZE, transport.m_iocpTcpClient.m_recvSizePacket, 0, 4, transport.m_iocpTcpClient);
                    try { socket.BeginReceive(sizeTransport.m_packet.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), sizeTransport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        transport.m_iocpTcpClient.Disconnect(); 
                        return;
                    }
                    transport.m_callBackObj.OnReceived(transport.m_iocpTcpClient, transport.m_packet);
                }
            }
          }

        
        /// Send callback function
        
        /// <param name="result">result</param>
        private static void onSent(IAsyncResult result)
        {
            PacketTransporter transport = result.AsyncState as PacketTransporter;
            Socket socket = transport.m_iocpTcpClient.m_client.Client;
 
            int sentSize=0;
            try { sentSize = socket.EndSend(result); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace); 
                transport.m_iocpTcpClient.Disconnect();
                transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR);
                return; 
            }
            if (sentSize == 0)
            {
                transport.m_iocpTcpClient.Disconnect();
                transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_CONNECTION_CLOSING);
                return;
            }
            if (sentSize < transport.m_size)
            {
                transport.m_offset = transport.m_offset + sentSize;
                transport.m_size = transport.m_size - sentSize;
                try { socket.BeginSend(transport.m_packet.GetPacket(), transport.m_offset, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), transport); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                    transport.m_iocpTcpClient.Disconnect();
                    transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR);
                    return;
                }
            }
            else
            {
                if (transport.m_packetType == PacketType.SIZE)
                {
                    transport.m_packet = transport.m_dataPacket;
                    transport.m_offset = 0;
                    transport.m_packetType = PacketType.DATA;
                    transport.m_size = transport.m_dataPacket.GetPacketByteSize();
                    try { socket.BeginSend(transport.m_packet.GetPacket(), 0, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), transport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        transport.m_iocpTcpClient.Disconnect();
                        transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR);
                        return;
                    }
                }
                else
                {
                    PacketTransporter delayedTransport = null;
                    lock (transport.m_iocpTcpClient.m_sendQueueLock)
                    {
                        Queue<PacketTransporter> sendQueue = transport.m_iocpTcpClient.m_sendQueue;
                        if (sendQueue.Count > 0)
                        {
                            delayedTransport = sendQueue.Dequeue();
                        }
                    }
                    if (delayedTransport != null)
                    {
                        try { socket.BeginSend(delayedTransport.m_packet.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), delayedTransport); }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                            transport.m_iocpTcpClient.Disconnect();
                            transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR);
                            return;
                        }
                    }
                    else
                    {
                        transport.m_iocpTcpClient.m_sendEvent.Unlock();
                    }
                    transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.SUCCESS);
                }
            }

        }

    }
}
