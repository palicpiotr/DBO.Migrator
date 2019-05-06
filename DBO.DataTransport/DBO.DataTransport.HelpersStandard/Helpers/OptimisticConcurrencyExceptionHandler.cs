using System;
using System.Data.SqlClient;
using System.Threading;
using System.Transactions;
using NLog;


namespace DBO.DataTransport.HelpersStandard.Helpers
{
    public class OptimisticConcurrencyExceptionHandler
    {
        public const int MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION = 2;
        public const int MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION = 5;
        public const int WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS = 100;
        private const string FAILED_PERMANENTLY_MESSAGE_FORMAT = "Failed permanently after {0} attempts.";

        private static readonly ILogger _logger = LogManager.GetLogger("OptimisticConcurrencyExceptionHandler", typeof(OptimisticConcurrencyExceptionHandler));

        private static void LogExceptionAsWarning(Exception ex, int numberOfAttempts)
        {
            _logger.Warn(ex, "ERROR, RETRYING (attempt#{0}):{1}{2}", numberOfAttempts, Environment.NewLine, ex);
        }

        /// <summary>
        /// Random interval on top of WaitTimeOnChangeConflictExceptionInMiliseconds
        /// </summary>
        /// <returns></returns>
        private static int GetRandomInterval()
        {
            Random random = new Random(1);
            int randomIntverval = random.Next(100, 350);
            return randomIntverval;
        }

        #region TryWriteToDatabase Actions

        public static void TryWriteToDatabase<TParam1>(Action<TParam1> dataAction,
                                                       TParam1 parameter1,
                                                       int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        dataAction(parameter1);
                        transactionScope.Complete();
                    }
                }
                else
                {
                    dataAction(parameter1);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     // ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    TryWriteToDatabase<TParam1>(dataAction, parameter1, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        public static void TryWriteToDatabase<TParam1, TParam2>(Action<TParam1, TParam2> dataAction,
                                                                TParam1 parameter1,
                                                                TParam2 parameter2,
                                                                int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        dataAction(parameter1, parameter2);
                        transactionScope.Complete();
                    }
                }
                else
                {
                    dataAction(parameter1, parameter2);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    TryWriteToDatabase<TParam1, TParam2>(dataAction, parameter1, parameter2, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        #endregion

        #region TryWriteToDatabase Func

        public static TReturnType TryWriteToDatabase<TParam1, TReturnType>(Func<TParam1, TReturnType> dataFunc,
                                                                           TParam1 parameter1,
                                                                           int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        TReturnType result = dataFunc(parameter1);
                        transactionScope.Complete();
                        return result;
                    }
                }
                else
                {
                    return dataFunc(parameter1);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    return TryWriteToDatabase<TParam1, TReturnType>(dataFunc, parameter1, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }
        public static TReturnType TryWriteToDatabase<TParam1, TParam2, TReturnType>(Func<TParam1, TParam2, TReturnType> dataFunc,
                                                                            TParam1 parameter1,
                                                                            TParam2 parameter2,
                                                                            int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        TReturnType result = dataFunc(parameter1, parameter2);
                        transactionScope.Complete();
                        return result;
                    }
                }
                else
                {
                    return dataFunc(parameter1, parameter2);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    return TryWriteToDatabase<TParam1, TParam2, TReturnType>(dataFunc, parameter1, parameter2, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        public static TReturnType TryWriteToDatabase<TParam1, TParam2, TParam3, TReturnType>(Func<TParam1, TParam2, TParam3, TReturnType> dataFunc,
                                                                                             TParam1 parameter1,
                                                                                             TParam2 parameter2,
                                                                                             TParam3 parameter3,
                                                                                             int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        TReturnType result = dataFunc(parameter1, parameter2, parameter3);
                        transactionScope.Complete();
                        return result;
                    }
                }
                else
                {
                    return dataFunc(parameter1, parameter2, parameter3);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    return TryWriteToDatabase<TParam1, TParam2, TParam3, TReturnType>(dataFunc, parameter1, parameter2, parameter3, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        public static TReturnType TryWriteToDatabase<TParam1, TParam2, TParam3, TParam4, TReturnType>(Func<TParam1, TParam2, TParam3, TParam4, TReturnType> dataFunc,
                                                                                                      TParam1 parameter1,
                                                                                                      TParam2 parameter2,
                                                                                                      TParam3 parameter3,
                                                                                                      TParam4 parameter4,
                                                                                                      int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        TReturnType result = dataFunc(parameter1, parameter2, parameter3, parameter4);
                        transactionScope.Complete();
                        return result;
                    }
                }
                else
                {
                    return dataFunc(parameter1, parameter2, parameter3, parameter4);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    return TryWriteToDatabase<TParam1, TParam2, TParam3, TParam4, TReturnType>(dataFunc, parameter1, parameter2, parameter3, parameter4, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        public static TReturnType TryWriteToDatabase<TParam1, TParam2, TParam3, TParam4, TParam5, TReturnType>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TReturnType> dataFunc,
                                                                                                      TParam1 parameter1,
                                                                                                      TParam2 parameter2,
                                                                                                      TParam3 parameter3,
                                                                                                      TParam4 parameter4,
                                                                                                      TParam5 parameter5,
                                                                                                      int numOfAttempts)
        {
            try
            {
                if (numOfAttempts > MAX_PROCESSING_ATTEMPTS_ON_CHANGE_CONFLICT_EXCEPTION)
                {
                    using (TransactionScope transactionScope = TransactionScopeInitialiser.GetNewDefaultTransactionScope())
                    {
                        TReturnType result = dataFunc(parameter1, parameter2, parameter3, parameter4, parameter5);
                        transactionScope.Complete();
                        return result;
                    }
                }
                else
                {
                    return dataFunc(parameter1, parameter2, parameter3, parameter4, parameter5);
                }
            }
            catch (Exception ex)
            {
                if ((ex is SqlException ||
                     //ex is ChangeConflictException ||
                     ex is TransactionAbortedException) &&
                     numOfAttempts < MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION)
                {
                    numOfAttempts++;
                    LogExceptionAsWarning(ex, numOfAttempts);
                    Thread.Sleep(WAIT_TIME_ON_CHANGE_CONFLICT_EXCEPTION_IN_MILISECONDS + GetRandomInterval());
                    return TryWriteToDatabase<TParam1, TParam2, TParam3, TParam4, TParam5, TReturnType>(dataFunc, parameter1, parameter2, parameter3, parameter4, parameter5, numOfAttempts);
                }
                else
                {
                    string errorMessage = string.Format(FAILED_PERMANENTLY_MESSAGE_FORMAT, MAX_PROCESSING_ATTEMPTS_ON_ANY_EXCEPTION);
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            }
        }

        #endregion
    }
}
