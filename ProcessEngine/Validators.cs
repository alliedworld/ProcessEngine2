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
    /// Validators Extension Functions
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Check if 'this' parameter is bot null
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static object NotNull(this object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException($"Parameter {name} cannot be null.");
            return obj;
        }
        /// <summary>
        /// Checks if 'this' parameter is not a empty string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object NotEmptyString(this object obj, string name)
        {
            if (obj != null && obj is string && string.Empty.Equals(((string)obj).Trim()))
                throw new ArgumentException($"String {name} cannot be empty.");
            return obj;
        }
    }
}
