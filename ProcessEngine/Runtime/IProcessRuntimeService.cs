/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

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
  */
using System;
using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Process Runtime Service
    /// </summary>
    public interface IProcessRuntimeService
    {
        IProcessRuntime Create(ProcessDefinition pd,IPropertySetCollection collection);

        /// <summary>
        /// Save or update the workflow
        /// </summary>
        /// <param name="runtime"></param>
        void Freeze(IProcessRuntime runtime, IPropertySetCollection collection);

        /// <summary>
        /// Try "unfreeze" the process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="runtime"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        bool TryUnfreeze(Guid processRuntimeId, out IProcessRuntime runtime, out StepRuntime nextStep, out IPropertySetCollection collection);
        /// <summary>
        /// Cancel the process
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool Cancel(Guid processRuntimeId, string reason);
        /// <summary>
        /// Retrieve Process Runtime summary
        /// </summary>
        /// <param name="processRuntimeId"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        bool TryGetProcessSummary(Guid processRuntimeId, out ProcessRuntimeSummary summary);
    }
}