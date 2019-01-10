﻿/*! 
@file IocpTcpClient.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief IocpTcpClient Interface
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

A IocpTcpClient Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Diagnostics;
using EpLibrary.cs;

namespace EpServerEngine.cs
{
    
    /// A IOCP TCP Client class.
    
    public sealed class IocpTcpClient : ThreadEx, INetworkClient
    {
        
        /// Actual TCP client
        
        private TcpClient m_client=new TcpClient();
        
        /// client options
        
        private ClientOps m_clientOps = null;

        
        /// general lock
        
        private Object m_generalLock = new Object();

        
        /// send lock
        
        private Object m_sendLock = new Object();
        
        /// send queue lock
        
        private Object m_sendQueueLock = new Object();
        
        /// send queue
        
        private Queue<PacketTransporter> m_sendQueue = new Queue<PacketTransporter>();

        
        /// callback object
        
        private INetworkClientCallback m_callBackObj=null;
        
        /// hostname
        
        private String m_hostName;
        
        /// port
        
        private String m_port;
        
        /// flag for nodelay
        
        private bool m_noDelay;
        
        /// wait time in millisecond
        
        private int m_waitTimeInMilliSec;

        
        /// connection time-out event
        
        private EventEx m_timeOutEvent = new EventEx(false, EventResetMode.AutoReset);
        
        /// send event
        
        private EventEx m_sendEvent = new EventEx();

        
        
        /// receive message size packet
        
        private Packet m_recvSizePacket = new Packet(null, 4);

        
        /// flag for connection check
        
        private bool m_isConnected = false;


        
        /// Default constructor
        
        public IocpTcpClient():base()
        {

        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
        public IocpTcpClient(IocpTcpClient b)
            : base(b)
        {
            m_clientOps = b.m_clientOps;
        }
        ~IocpTcpClient()
        {
            if (IsConnectionAlive())
                Disconnect();
        }

        
        /// Return hostname
        
        /// <returns>hostname</returns>
        public String GetHostName()
        {
            lock (m_generalLock)
            {
                return m_hostName;
            }
        }

        
        /// Return port
        
        /// <returns>port</returns>
        public String GetPort()
        {
            lock (m_generalLock)
            {
                return m_port;
            }
            
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
        
        /// Make the connection to the server and start receiving
        
        protected override void execute()
        {
            ConnectStatus status = ConnectStatus.SUCCESS;
            try
            {
                lock (m_generalLock)
                {
                    if (IsConnectionAlive())
                    {
                        status = ConnectStatus.FAIL_ALREADY_CONNECTED;
                        throw new CallbackException();
                    }

                    m_callBackObj = m_clientOps.callBackObj;
                    m_hostName = m_clientOps.hostName;
                    m_port = m_clientOps.port;
                    m_noDelay = m_clientOps.noDelay;
                    m_waitTimeInMilliSec = m_clientOps.waitTimeInMilliSec;

                    if (m_hostName == null || m_hostName.Length == 0)
                    {
                        m_hostName = ServerConf.DEFAULT_HOSTNAME;
                    }

                    if (m_port == null || m_port.Length == 0)
                    {
                        m_port = ServerConf.DEFAULT_PORT;
                    }


                    m_client.NoDelay = m_noDelay;

                    m_client.Client.BeginConnect(m_hostName, Convert.ToInt32(m_port), new AsyncCallback(IocpTcpClient.onConnected), this);
                    if (m_timeOutEvent.WaitForEvent(m_waitTimeInMilliSec))
                    {
                        if (!m_client.Connected)
                        {
                            status = ConnectStatus.FAIL_SOCKET_ERROR;
                            throw new CallbackException();
                        }
                        m_isConnected = true;
                        if (m_callBackObj != null)
                        {
                            Thread t = new Thread(delegate()
                            {
                                m_callBackObj.OnConnected(this, ConnectStatus.SUCCESS);
                            });
                            t.Start();
                        }
                           
                    }
                    else
                    {
                        m_client.Close();
                        status = ConnectStatus.FAIL_TIME_OUT;
                        throw new CallbackException();
                    }
              
                }
            }
            catch(CallbackException)
            {
                if (m_callBackObj != null)
                {
                    Thread t = new Thread(delegate()
                    {
                        m_callBackObj.OnConnected(this, status);
                    });
                    t.Start();
                    
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (m_callBackObj != null)
                {
                    Thread t = new Thread(delegate()
                    {
                        m_callBackObj.OnConnected(this, ConnectStatus.FAIL_SOCKET_ERROR);
                    });
                    t.Start();
                   
                }
                return;
            }
            startReceive();

        }
        
        /// Connect to the server with given options
        
        /// <param name="ops">options for client</param>
        public void Connect(ClientOps ops)
        {
            lock (m_generalLock)
            {
                if (IsConnectionAlive())
                    return;
            }
            if (ops == null)
                ops = ClientOps.defaultClientOps;
            if (ops.callBackObj == null)
                throw new NullReferenceException("callBackObj is null!");
            lock (m_generalLock)
            {
                m_clientOps = ops;
            }
            Start();
      
        }

        
        /// Connection callback function
        
        /// <param name="result">result</param>
        private static void onConnected(IAsyncResult result)
        {
            IocpTcpClient tcpclient = result.AsyncState as IocpTcpClient;
     
            try { tcpclient.m_client.Client.EndConnect(result); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                tcpclient.m_timeOutEvent.SetEvent();
                return;
            }
            tcpclient.m_timeOutEvent.SetEvent();
            //if (tcpclient.m_callBackObj != null) 
            //    tcpclient.m_callBackObj.OnConnected(tcpclient, ConnectStatus.SUCCESS);
            return;
          
        }

        
        /// Disconnect from the server
        
        public void Disconnect()
        {
            lock (m_generalLock)
            {
                if (!IsConnectionAlive())
                    return;
                m_client.Close();
                m_isConnected = false;
            }

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
            try
            {
                return m_client.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                return false;
            }

        }

        
        /// Send given packet to the server
        
        /// <param name="packet">packet to send</param>
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
                Packet sendSizePacket = new Packet(null, 4, false);
                PacketTransporter transport = new PacketTransporter(PacketType.SIZE, sendSizePacket, 0, 4, this, packet);
                sendSizePacket.SetPacket(BitConverter.GetBytes(packet.GetPacketByteSize()), 4);
                if (m_sendEvent.TryLock())
                {
                    try { m_client.Client.BeginSend(sendSizePacket.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpClient.onSent), transport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        if (m_callBackObj != null)
                            m_callBackObj.OnSent(this, SendStatus.FAIL_SOCKET_ERROR);
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
            
            /// client
            
            public IocpTcpClient m_iocpTcpClient;
            
            /// packet type
            
            public PacketType m_packetType;
            
            /// callback object
            
            public INetworkClientCallback m_callBackObj;
            
            /// Default constructor
            
            /// <param name="packetType">packet type</param>
            /// <param name="packet">packet</param>
            /// <param name="offset">offset</param>
            /// <param name="size">size of packet in byte</param>
            /// <param name="iocpTcpClient">client</param>
            /// <param name="dataPacket">data packet for send</param>
            public PacketTransporter(PacketType packetType,Packet packet, int offset, int size, IocpTcpClient iocpTcpClient,Packet dataPacket=null)
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
            try { m_client.Client.BeginReceive(m_recvSizePacket.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpClient.onReceived), transport); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                Disconnect(); return;
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
                transport.m_iocpTcpClient.Disconnect(); return;
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
                try{socket.BeginReceive(transport.m_packet.GetPacket(), transport.m_offset, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpClient.onReceived), transport);}
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                    transport.m_iocpTcpClient.Disconnect(); return;
                }
            }
            else
            {
                if (transport.m_packetType == PacketType.SIZE)
                {
                    int shouldReceive = BitConverter.ToInt32(transport.m_packet.GetPacket(), 0);
                    Packet recvPacket = new Packet(null, shouldReceive);
                    PacketTransporter dataTransport = new PacketTransporter(PacketType.DATA, recvPacket, 0, shouldReceive, transport.m_iocpTcpClient);
                    try{socket.BeginReceive(recvPacket.GetPacket(), 0, shouldReceive, SocketFlags.None, new AsyncCallback(IocpTcpClient.onReceived), dataTransport);}
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace); 
                        transport.m_iocpTcpClient.Disconnect(); return;
                    }
                }
                else
                {
                    PacketTransporter sizeTransport = new PacketTransporter(PacketType.SIZE, transport.m_iocpTcpClient.m_recvSizePacket, 0, 4, transport.m_iocpTcpClient);
                    try { socket.BeginReceive(sizeTransport.m_packet.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpClient.onReceived), sizeTransport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        transport.m_iocpTcpClient.Disconnect(); return;
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
                return; }
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
                try { socket.BeginSend(transport.m_packet.GetPacket(), transport.m_offset, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpClient.onSent), transport); }
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
                    try { socket.BeginSend(transport.m_packet.GetPacket(), 0, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpClient.onSent), transport); }
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
                        try { socket.BeginSend(delayedTransport.m_packet.GetPacket(), 0, 4, SocketFlags.None, new AsyncCallback(IocpTcpClient.onSent), delayedTransport); }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                            transport.m_callBackObj.OnSent(transport.m_iocpTcpClient, SendStatus.SUCCESS);
                            delayedTransport.m_iocpTcpClient.Disconnect();
                            delayedTransport.m_callBackObj.OnSent(delayedTransport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR);
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
