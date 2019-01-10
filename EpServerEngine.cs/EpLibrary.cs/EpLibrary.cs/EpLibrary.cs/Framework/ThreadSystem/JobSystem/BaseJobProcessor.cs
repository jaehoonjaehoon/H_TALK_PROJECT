/*! 
@file BaseJobProcessor.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eplibrary.cs>
@date April 01, 2014
@brief BaseJobProcessor Interface
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

A BaseJobProcessor Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EpLibrary.cs
{
    
    /// Enumeration for Job Processor Status
    
    public enum JobProcessorStatus
    {
        
        /// Job Processor never entered to the scheduler
        
        NONE = 0,
        
        /// Job Processor is in the Pending State
        
        PENDING,
        
        /// Job Processor is in the Process
        
        IN_PROCESS,
        
        /// Job Processor is finished working
        
        DONE,
        
        /// Job Processor is suspended by other processor
        
        SUSPENDED,
        
        /// Job Processor terminated due to Thread Error or Internal Error
        
        INCOMPLETE,
        
        /// Job PRocessor TImed Out
        
        TIMEOUT,
    };

    
    /// A base class for Job Processing Objects.
    
    public abstract class BaseJobProcessor
    {


        
        /// Process the job given, subclasses must implement this function.
        
        /// <param name="workerThread">The worker thread which called the DoJob.</param>
        /// <param name="data">The job given to this object.</param>
        public abstract void DoJob(BaseWorkerThread workerThread, BaseJob data);

        
        /// Handles when Job Status Changed
        
        /// <param name="status">The Status of the Job</param>
        /// <remarks>Subclass should overwrite this function!!</remarks>
		protected virtual void handleReport(JobProcessorStatus status)
        {

        }

        
        /// Default constructor
        
        protected BaseJobProcessor()
        {
            m_status = JobProcessorStatus.NONE;
        }

        
        /// Default copy constructor
        
        /// <param name="b">the object to copy from</param>
		protected BaseJobProcessor(BaseJobProcessor b)
		{
			m_status=b.m_status;
		}

        
        /// Call Back Function When Job's Status Changed.
        
        /// <param name="status">The Status of the Job</param>
        private void JobProcessorReport(JobProcessorStatus status)
        {
            handleReport(status);
            m_status = status;
        }


        
        /// current Job Processor Status
        
		private JobProcessorStatus m_status;
    }
}
