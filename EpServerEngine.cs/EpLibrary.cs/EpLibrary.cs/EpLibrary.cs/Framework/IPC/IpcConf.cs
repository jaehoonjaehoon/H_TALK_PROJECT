/*! 
@file IpcConf.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief IpcConf Interface
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

A IpcConf Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpLibrary.cs
{
    
    /// Server start status
    
    public enum IpcStartStatus
    {
        
        /// Success
        
        SUCCESS=0,
        
        /// Failed to create pipe
        
        FAIL_PIPE_CREATE_FAILED,
    }
	
    /// Connect Status
	
    public enum IpcConnectStatus
    {
		
        /// Success
		
		SUCCESS=0,
        
        /// Failed to wait for connection
        
        FAIL_WAIT_FOR_CONNECTION_FAILED,
		
        /// Pipe open failed
		
		FAIL_PIPE_OPEN_FAILED,
		
        /// ReadMode Set failed
		
		FAIL_SET_READ_MODE_FAILED,
		
        /// Read failed
		
		FAIL_READ_FAILED,
		
        /// Timed Out
		
		FAIL_TIME_OUT,
	}


	
    /// Write Status
	
	public enum IpcWriteStatus{
		
        /// Success
		
		SUCCESS=0,
		
        /// Send failed
		
		FAIL_WRITE_FAILED,
	}

    
    /// Pipe write element
    
	public class PipeWriteElem{
        
        /// offset of start of data
        
        public int m_offset;
        
        /// /// Byte size of the data
        
        public int m_dataSize;
        
        /// Data buffer
        
        public byte[] m_data;

        
        /// Default constructor
        
		public PipeWriteElem()
        {
            m_dataSize=0;
            m_offset = 0;
            m_data=null;
        }

        
        /// Default Constructor
        
        /// <param name="data">the byte size of the data</param>
        /// <param name="offset">offset of the byte to start write</param>
        /// <param name="dataSize">byte size of the data to write</param>
        public PipeWriteElem(byte[] data, int offset,int dataSize)
        {
            m_offset = offset;
            m_dataSize=dataSize;
            m_data = data;
        }

	}
    
    /// IPC configuration class
    
    public class IpcConf
    {
        
        /// Unlimited instance of pipe
        
        public const int DEFAULT_PIPE_INSTANCES = 255;
        
        /// Default write buffer size
        
        public const int DEFAULT_WRITE_BUF_SIZE = 4096;
        
        /// Default read buffer size
        
        public const int DEFAULT_READ_BUF_SIZE = 4096;
    }
}
