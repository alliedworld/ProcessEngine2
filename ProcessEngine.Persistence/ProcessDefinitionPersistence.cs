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
using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Persistent Process Definition
    /// </summary>
    public class ProcessDefinitionPersistence
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Process Definition Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Process Defnition Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// MD5 of the definition
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// Serialized JSON body
        /// </summary>
        public string JsonProcessDefinition { get; set; }
        /// <summary>
        /// Last Modified
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Flow Id
        /// </summary>
        public string FlowId { get; set; }
        /// <summary>
        /// List of accounts that
        /// </summary>
        public virtual ICollection<ProcessDefinitionAccount> Accounts { get; set; }
    }
}