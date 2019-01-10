/*! 
@file ClientInterface.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief ClientInterface Interface
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

A ClientInterface Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EpServerEngine.cs
{
    public sealed class ClientOps{
		public INetworkClientCallback callBackObj;
        public String hostName;
		public String port;
        public bool noDelay;
        public int waitTimeInMilliSec;

        public ClientOps()
		{
			callBackObj=null;
			hostName=ServerConf.DEFAULT_HOSTNAME;
			port=ServerConf.DEFAULT_PORT;
            noDelay = true;
            waitTimeInMilliSec = Timeout.Infinite;
		}
        public ClientOps(INetworkClientCallback callBackObj, String hostName, String port, bool noDelay = true, int waitTimeInMilliSec = Timeout.Infinite)
        {
            this.callBackObj = callBackObj;
            this.hostName = hostName;
            this.port = port;
            this.noDelay = noDelay;
            this.waitTimeInMilliSec = waitTimeInMilliSec;
        }
		public static ClientOps defaultClientOps=new ClientOps();
	};

    public interface INetworkClient
    {
        String GetHostName();
        String GetPort();

        void Connect(ClientOps ops);

        void Disconnect();

        bool IsConnectionAlive();

        void Send(Packet packet);

        
    }

	public interface INetworkClientCallback{
        void OnConnected(INetworkClient client, ConnectStatus status);

	    void OnReceived(INetworkClient client, Packet receivedPacket);

        void OnSent(INetworkClient client, SendStatus status);

        void OnDisconnect(INetworkClient client);
	};
}
