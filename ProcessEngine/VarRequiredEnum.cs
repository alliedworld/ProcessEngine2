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

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Enumeration that defines step required variables
    /// </summary>
    [Flags]
    public enum VarRequiredEnum
    {
        /// <summary>
        /// a variable value is optional
        /// </summary>
        None = 0,
        /// <summary>
        /// a variable value required to be set before a step body will be executed
        /// </summary>
        OnEntry=1,
        /// <summary>
        /// a variable value should be set by step body execution
        /// </summary>
        OnExit=2,
        /// <summary>
        /// a variable value is read-only in the step
        /// </summary>
        ReadOnly=4
    }
}