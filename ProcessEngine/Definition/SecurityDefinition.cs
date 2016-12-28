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
using System.Runtime.Serialization;
using KlaudWerk.ProcessEngine.Builder;

namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract(Name="security")]
    public class SecurityDefinition:IProcessDefinitionVisitable
    {
        [DataMember(Name = "acc")]
        public string Account { get; set; }
        [DataMember(Name="type")]
        public AccountTypeEnum AccountType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType"></param>
        public SecurityDefinition(string account, AccountTypeEnum accountType)
        {
            Account = account;
            AccountType = accountType;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SecurityDefinition()
        {
        }

        public void Accept(IProcessDefinitionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}