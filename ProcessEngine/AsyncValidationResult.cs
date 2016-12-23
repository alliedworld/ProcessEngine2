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