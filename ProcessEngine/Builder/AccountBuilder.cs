using System.Collections.Generic;

namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Account Builder
    /// </summary>
    public class AccountBuilder
    {
        private readonly VariableBuilder _parenty;
        private readonly List<string> _accounts = new List<string>();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        public AccountBuilder(VariableBuilder parent, IEnumerable<string> accounts)
        {
            _parenty = parent;
            _accounts.AddRange(_accounts);
        }
        /// <summary>
        /// Add new account
        /// </summary>
        /// <param name="accountName">name of the security account</param>
        /// <returns></returns>
        public AccountBuilder Add(string accountName)
        {
            if (!_accounts.Contains(accountName))
                _accounts.Add(accountName);
            return this;
        }
        /// <summary>
        /// Remove an account
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public AccountBuilder Remove(string accountName)
        {
            _accounts.Remove(accountName);
            return this;
        }
        /// <summary>
        /// Clear the account list
        /// </summary>
        /// <returns></returns>
        public AccountBuilder Clear()
        {
            _accounts.Clear();
            return this;
        }
        /// <summary>
        /// Done building
        /// </summary>
        /// <returns></returns>
        public VariableBuilder Done()
        {
            _parenty.SetAccountValues(_accounts);
            return _parenty;
        }

    }
}