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
    /// The result of Process Validation
    /// </summary>
    public class ProcessValidationResult
    {
        /// <summary>
        /// The type of the artifact
        /// </summary>
        public enum ItemEnum
        {
            Process=1,
            Step,
            Link,
            Script
        }
        /// <summary>
        /// Gets the artifact.
        /// </summary>
        /// <value>
        /// The artifact.
        /// </value>
        public ItemEnum Artifact { get; }
        /// <summary>
        /// Gets the artifact identifier.
        /// </summary>
        /// <value>
        /// The artifact identifier.
        /// </value>
        public string ArtifactId { get; }
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="artifactId"></param>
        /// <param name="message"></param>
        public ProcessValidationResult(ItemEnum artifact, string artifactId, string message)
        {
            Artifact = artifact;
            ArtifactId = artifactId;
            Message = message;
        }
    }
}