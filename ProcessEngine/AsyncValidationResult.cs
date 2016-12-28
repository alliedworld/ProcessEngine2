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

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Async Validatio Result return
    /// </summary>
    public class AsyncValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="AsyncValidationResult"/> is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public bool Valid { get; }
        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public string[] Messages { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationResult"/> class.
        /// </summary>
        public AsyncValidationResult() : this(true, new string[] { }) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationResult"/> class.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public AsyncValidationResult(params string[] messages) : this(false, messages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationResult"/> class.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="messages">The messages.</param>
        public AsyncValidationResult(bool isValid,params string[] messages)
        {
            Valid = isValid;
            Messages = messages;
        }
    }
}