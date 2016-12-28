using System;

namespace KlaudWerk.ProcessEngine.Persistence
{
    /// <summary>
    /// Class that represents security account
    /// </summary>
    public class AccountData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the source system.
        /// </summary>
        /// <value>
        /// The source system.
        /// </value>
        public string SourceSystem { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the account.
        /// The type can be:
        /// 1 - User
        /// 2 - Group
        /// 3 - Role
        /// </summary>
        /// <value>
        /// The type of the account.
        /// </value>
        public int AccountType { get; set; }
    }
}