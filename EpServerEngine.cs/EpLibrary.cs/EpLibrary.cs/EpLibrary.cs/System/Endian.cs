/*! 
@file Endian.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief Endian Interface
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

A Endian Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;

namespace EpLibrary.cs
{
    
    /// A class for Endian.
    
    public class Endian
    {
        
        public static double Swap(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
        
        public static float Swap(float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        
        public static int Swap(int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint Swap(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static long Swap(long val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static ulong Swap(ulong val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        
        public static short Swap(short val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        
        public static ushort Swap(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        
        public static short HostToNetWorkOrder(short host)
        {
            return IPAddress.HostToNetworkOrder(host);
        }

        
        public static int HostToNetWorkOrder(int host)
        {
            return IPAddress.HostToNetworkOrder(host);
        }

        
        public static long HostToNetWorkOrder(long host)
        {
            return IPAddress.HostToNetworkOrder(host);
        }

        
        public static short NetworkToHostOrder(short host)
        {
            return IPAddress.NetworkToHostOrder(host);
        }

        public static int NetworkToHostOrder(int host)
        {
            return IPAddress.NetworkToHostOrder(host);
        }

        public static long NetworkToHostOrder(long host)
        {
            return IPAddress.NetworkToHostOrder(host);
        }

        
        public static bool IsLittleEndian()
        {
            return BitConverter.IsLittleEndian;
        }

        public static bool IsBigEndian()
        {
            return !BitConverter.IsLittleEndian;
        }
    }
}
