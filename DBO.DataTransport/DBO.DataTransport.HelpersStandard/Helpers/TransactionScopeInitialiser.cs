using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;


namespace DBO.DataTransport.HelpersStandard.Helpers
{
    public class TransactionScopeInitialiser
    {
        private const int DEFAULT_TRANSACTION_TIMROUT_IN_SECONDS = 5;

        public static TransactionScope GetNewDefaultTransactionScope()
        {
            TransactionOptions transactionOptions = GetDefaultTransactionOptions();
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        public static TransactionOptions GetDefaultTransactionOptions()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = new TimeSpan(0, 0, DEFAULT_TRANSACTION_TIMROUT_IN_SECONDS)
            };
            return transactionOptions;
        }

        public static IsolationLevel GetIsolationLevel()
        {
            return IsolationLevel.RepeatableRead;
        }
    }
}
