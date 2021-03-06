﻿/*! 
@file PacketSerializer.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epserverengine.cs>
@date April 01, 2014
@brief PacketSerializer Interface
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

A PacketSerializer Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System.Reflection;
using EpLibrary.cs;

namespace EpServerEngine.cs
{
    public enum SerializerMode
    {
        DEFAULT,
        ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION,
        SILVERLIGHT_SERIALIZER
    }
    public sealed class PacketSerializer<PacketStruct> where PacketStruct : class,ISerializable
    {
        sealed class AllowAllAssemblyVersionDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                String currentAssembly = Assembly.GetAssembly(typeof(PacketStruct)).FullName;
                assemblyName = currentAssembly;
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeof(PacketStruct).FullName, assemblyName));
                return typeToDeserialize;
            }
        }

        private MemoryStream m_stream = null;
        private BinaryFormatter m_formatter = new BinaryFormatter();
        
        private Object m_packetContainerLock = new Object();

        public SerializerMode Mode
        {
            get;
            set;
        }

        public PacketSerializer(PacketStruct packet = null, SerializerMode serializerMode = SerializerMode.SILVERLIGHT_SERIALIZER)
        {
            m_formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            
            m_stream = new MemoryStream();
            Mode = serializerMode;
            switch (Mode)
            {
                case SerializerMode.DEFAULT:
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                    m_formatter.Serialize(m_stream, packet);
                    break;
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    SilverlightSerializer.Serialize(packet, m_stream);
                    break;
            }
            
        }

        public PacketSerializer(byte[] rawData, SerializerMode serializerMode = SerializerMode.SILVERLIGHT_SERIALIZER)
        {
            m_formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            Mode = serializerMode;
            switch (Mode)
            {
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                    m_formatter.Binder = new AllowAllAssemblyVersionDeserializationBinder();
                    break;
                case SerializerMode.DEFAULT:
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    break;
            }
            m_stream = new MemoryStream(rawData);
        }

        public PacketSerializer(byte[] rawData, int offset, int count, SerializerMode serializerMode = SerializerMode.SILVERLIGHT_SERIALIZER)
        {
            m_formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            Mode = serializerMode;
            switch (Mode)
            {
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                    m_formatter.Binder = new AllowAllAssemblyVersionDeserializationBinder();
                    break;
                case SerializerMode.DEFAULT:
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    break;
            }
            m_stream = new MemoryStream(rawData, offset, count);
        }

        public PacketSerializer(PacketSerializer<PacketStruct> orig)
        {
            m_formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            Mode = orig.Mode;
            switch (Mode)
            {
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                    m_formatter.Binder = new AllowAllAssemblyVersionDeserializationBinder();
                    break;
                case SerializerMode.DEFAULT:
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    break;
            }
            m_stream=orig.m_stream;
        }


        public PacketStruct GetPacket()
        {
            m_stream.Seek(0, SeekOrigin.Begin);
            PacketStruct retPacket=null;
            switch (Mode)
            {
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                case SerializerMode.DEFAULT:
                    retPacket = (PacketStruct)m_formatter.Deserialize(m_stream);
                    break;
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    retPacket = (PacketStruct)SilverlightSerializer.Deserialize(m_stream);
                    break;
            }
            return retPacket;
        }

        public byte[] GetPacketRaw()
        {
            //return m_stream.ToArray();
            return m_stream.GetBuffer();

        }
        public long GetPacketByteSize()
        {
            return m_stream.Length;   
        }

        public void SetPacket(PacketStruct packet)
        {
            m_stream = new MemoryStream();
            switch (Mode)
            {
                case SerializerMode.DEFAULT:
                case SerializerMode.ALLOW_ALL_ASSEMBLY_VERSION_DESERIALIZATION:
                    m_formatter.Serialize(m_stream, packet);
                    break;
                case SerializerMode.SILVERLIGHT_SERIALIZER:
                    SilverlightSerializer.Serialize(packet, m_stream);
                    break;
            }
        }

        public void SetPacket(byte[] rawData)
        {
            m_stream = new MemoryStream(rawData);

        }

        public void SetPacket(byte[] rawData, int offset,int count)
        {
            m_stream = new MemoryStream(rawData, offset, count);

        }

    }
}
