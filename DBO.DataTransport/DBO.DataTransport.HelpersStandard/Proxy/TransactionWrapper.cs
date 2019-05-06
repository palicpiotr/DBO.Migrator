using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace DBO.DataTransport.HelpersStandard.Proxy
{
    public class TransactionWrapper : IDisposable
    {
        private IDbContextTransaction _transaction;

        public TransactionWrapper()
        {

        }

        public void Init(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public TransactionWrapper(IDbContextTransaction transaction)
        {
            Init(transaction);
        }

        public virtual void Commit()
        {
            _transaction.Commit();
        }

        public virtual void Rollback()
        {
            _transaction.Rollback();
        }

        public virtual void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
