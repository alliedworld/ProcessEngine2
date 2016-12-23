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