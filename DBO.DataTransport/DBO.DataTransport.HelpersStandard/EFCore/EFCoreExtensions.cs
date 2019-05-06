using DBO.DataTransport.HelpersStandard.Async;
using DBO.DataTransport.HelpersStandard.Attributes;
using DBO.DataTransport.HelpersStandard.Reflection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DBO.DataTransport.HelpersStandard.EFCore
{
    public static class EFCoreExtensions
    {
        private static readonly Dictionary<string, HashSet<string>> _tableTypesColumns = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Creates an initial DbCommand object based on a stored procedure name
        /// </summary>
        /// <param name="context">target database context</param>
        /// <param name="storedProcName">target procedure name</param>
        /// <param name="prependDefaultSchema">Prepend the default schema name to <paramref name="storedProcName"/> if explicitly defined in <paramref name="context"/></param>
        /// <param name="commandTimeout">Command timeout in seconds. Default is 30.</param>
        /// <returns></returns>
        public static DbCommand LoadStoredProc(this DbContext context, string storedProcName, bool prependDefaultSchema = true, short commandTimeout = 30)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();

            cmd.CommandTimeout = commandTimeout;

            if (prependDefaultSchema)
            {
                var schemaName = context.Model.Relational().DefaultSchema;
                if (schemaName != null)
                {
                    storedProcName = $"{schemaName}.{storedProcName}";
                }
            }

            cmd.CommandText = storedProcName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            return cmd;
        }

        /// <summary>
        /// Creates a DbParameter object and adds it to a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue, Action<DbParameter> configureParam = null)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != System.Data.CommandType.StoredProcedure)
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue ?? DBNull.Value;
            configureParam?.Invoke(param);
            cmd.Parameters.Add(param);
            return cmd;
        }

        /// <summary>
        /// Creates a DbParameter object and adds it to a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, Action<DbParameter> configureParam = null)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != System.Data.CommandType.StoredProcedure)
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            configureParam?.Invoke(param);
            cmd.Parameters.Add(param);
            return cmd;
        }

        /// <summary>
        /// Creates a DbParameter object based on the SqlParameter and adds it to a DbCommand.
        /// This enabled the ability to provide custom types for SQL-parameters.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, SqlParameter parameter)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != System.Data.CommandType.StoredProcedure)
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            //var param = cmd.CreateParameter();
            //param.ParameterName = paramName;
            //configureParam?.Invoke(param);
            cmd.Parameters.Add(parameter);

            return cmd;
        }

        public class SprocResults
        {

            //  private DbCommand _command;
            private DbDataReader _reader;

            public SprocResults(DbDataReader reader)
            {
                // _command = command;
                _reader = reader;
            }

            public IList<T> ReadToList<T>()
            {
                return MapToList<T>(_reader);
            }

            public T? ReadToValue<T>() where T : struct
            {
                return MapToValue<T>(_reader);
            }

            public Task<bool> NextResultAsync()
            {
                return _reader.NextResultAsync();
            }

            public Task<bool> NextResultAsync(CancellationToken ct)
            {
                return _reader.NextResultAsync(ct);
            }

            public bool NextResult()
            {
                return _reader.NextResult();
            }

            /// <summary>
            /// Retrieves the column values from the stored procedure and maps them to <typeparamref name="T"/>'s properties
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="dr"></param>
            /// <returns>IList<<typeparamref name="T"/>></returns>
            private IList<T> MapToList<T>(DbDataReader dr)
            {
                var objList = new List<T>();
                var props = typeof(T).GetRuntimeProperties().ToList();

                var colMapping = dr.GetColumnSchema()
                                   .Where(x =>
                                        props.Any(y =>
                                            y.Name.ToLower() == x.ColumnName.ToLower()))
                                   .ToDictionary(key => key.ColumnName.ToLower());

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        if (IsPrimitiveOrNullablePrimitive(obj.GetType()))
                        {
                            var val = dr.GetValue(0);
                            if (val != DBNull.Value)
                                obj = (T)val;
                        }
                        else
                        {
                            foreach (var prop in props)
                            {
                                if (colMapping.ContainsKey(prop.Name.ToLower()))
                                {
                                    var column = colMapping[prop.Name.ToLower()];
                                    if (column?.ColumnOrdinal != null)
                                    {
                                        var val = dr.GetValue(column.ColumnOrdinal.Value);
                                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                                    }
                                }
                            }
                        }
                        if (!EqualityComparer<T>.Default.Equals(obj, default(T)))
                            objList.Add(obj);
                    }
                }
                return objList;
            }

            /// <summary>
            /// Checks type of current type for primitive
            /// </summary>
            /// <param name="objType">Current type of oject</param>
            /// <returns>true/false</returns>
            private bool IsPrimitiveOrNullablePrimitive(Type objType)
            {
                return objType.IsPrimitive ||
                       objType.IsValueType ||
                       (objType == typeof(string)) ||
                       Convert.GetTypeCode(objType) != TypeCode.Object ||
                       objType.IsGenericType &&
                       objType.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            /// <summary>
            ///Attempts to read the first value of the first row of the resultset.
            /// </summary>
            private T? MapToValue<T>(DbDataReader dr) where T : struct
            {
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        return dr.IsDBNull(0) ? new T?() : new T?(dr.GetFieldValue<T>(0));
                    }
                }
                return new T?();
            }
        }

        /// <summary>
        /// Executes a DbDataReader and returns a list of mapped column values to the properties of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void ExecuteStoredProc(this DbCommand command, Action<SprocResults> handleResults, System.Data.CommandBehavior commandBehaviour = System.Data.CommandBehavior.Default, bool manageConnection = true)
        {
            if (handleResults == null)
            {
                throw new ArgumentNullException(nameof(handleResults));
            }

            using (command)
            {
                if (manageConnection && command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader(commandBehaviour))
                    {
                        var sprocResults = new SprocResults(reader);
                        // return new SprocResults();
                        handleResults(sprocResults);
                    }
                }
                finally
                {
                    if (manageConnection)
                    {
                        command.Connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Executes a DbDataReader asynchronously and returns a list of mapped column values to the properties of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async static Task ExecuteStoredProcAsync(this DbCommand command, Action<SprocResults> handleResults, System.Data.CommandBehavior commandBehaviour = System.Data.CommandBehavior.Default, CancellationToken ct = default(CancellationToken), bool manageConnection = true)
        {
            if (handleResults == null)
            {
                throw new ArgumentNullException(nameof(handleResults));
            }

            using (command)
            {
                if (manageConnection && command.Connection.State == System.Data.ConnectionState.Closed)
                    await command.Connection.OpenAsync(ct).ConfigureAwait(false);
                try
                {
                    using (var reader = await command.ExecuteReaderAsync(commandBehaviour, ct).ConfigureAwait(false))
                    {
                        var sprocResults = new SprocResults(reader);
                        handleResults(sprocResults);
                    }
                }
                finally
                {
                    if (manageConnection)
                    {
                        command.Connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Executes a non-query.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandBehaviour"></param>
        /// <param name="manageConnection"></param>
        /// <returns></returns>
        public static int ExecuteStoredNonQuery(this DbCommand command, System.Data.CommandBehavior commandBehaviour = System.Data.CommandBehavior.Default, bool manageConnection = true)
        {
            int numberOfRecordsAffected = -1;

            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                {
                    command.Connection.Open();
                }

                try
                {
                    numberOfRecordsAffected = command.ExecuteNonQuery();
                }
                finally
                {
                    if (manageConnection)
                    {
                        command.Connection.Close();
                    }
                }
            }

            return numberOfRecordsAffected;
        }

        /// <summary>
        /// Executes a non-query asynchronously.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandBehaviour"></param>
        /// <param name="ct"></param>
        /// <param name="manageConnection"></param>
        /// <returns></returns>
        public async static Task<int> ExecuteStoredNonQueryAsync(this DbCommand command, System.Data.CommandBehavior commandBehaviour = System.Data.CommandBehavior.Default, CancellationToken ct = default(CancellationToken), bool manageConnection = true)
        {
            int numberOfRecordsAffected = -1;

            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                {
                    await command.Connection.OpenAsync(ct).ConfigureAwait(false);
                }

                try
                {
                    numberOfRecordsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                finally
                {
                    if (manageConnection)
                    {
                        command.Connection.Close();
                    }
                }
            }

            return numberOfRecordsAffected;
        }

        public static async Task<SqlDataReader> ExecuteTableValueProcedureAsync<T>(this DbContext context, IEnumerable<T> data, string procedureName, string paramName, string typeName, CancellationToken token)
        {
            var table = data.ToDataTable();
            return await context.ExecuteTableValueProcedureAsync(table, procedureName, paramName, typeName, token);
        }

        public static async Task<SqlDataReader> ExecuteTableValueProcedureAsync(this DbContext context, DataTable data, string procedureName,
            string paramName, string typeName, CancellationToken token)
        {

            var names = await context.GetTypeNames(typeName, token);

            foreach (var excludedColumn in CheckColumns(data, names))
                data.Columns.Remove(excludedColumn);

            // create parameter 
            SqlParameter parameter = new SqlParameter(paramName, data)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = typeName
            };

            // execute sp sql 
            var connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString);
            var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120,

            };
            command.Parameters.Add(parameter);
            connection.Open();
            var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, token);
            // execute sql 
            return reader;
        }

        private static readonly KeyedSemaphoreSlim _lock = new KeyedSemaphoreSlim();

        private static async Task<HashSet<string>> GetTypeNames(this DbContext context, string typeName, CancellationToken token)
        {
            if (_tableTypesColumns.TryGetValue(typeName, out var names))
                return names;
            using (await _lock.WaitAsync(typeName, token))
            {
                names = new HashSet<string>();
                var command = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "dbo.GetTableTypeColumns",
                    CommandTimeout = 120,
                    Connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString),
                };
                command.Parameters.AddWithValue("@TypeName", typeName);
                command.Connection.Open();
                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, token))
                    while (reader.Read())
                        names.Add(reader.GetString(0));
                try
                {
                    return _tableTypesColumns[typeName] = names;
                }
                catch (NullReferenceException)
                {
                    Thread.Sleep(300);
                    return _tableTypesColumns[typeName] = names;
                }
            }
        }

        private static List<string> CheckColumns(DataTable data, HashSet<string> names)
        {
            var res = new List<string>();
            foreach (DataColumn column in data.Columns)
                if (!names.Contains(column.ColumnName.ToLower()))
                    res.Add(column.ColumnName);
            return res;
        }

        public static async Task<SqlDataReader> ExecuteSelectionsPricesTableValuesProcedureAsync<TS, TP>(this DbContext context, string procedureName, CancellationToken token, DataToExecute<TS> data1, DataToExecute<TP> data2)
        {
            string sql = $"EXEC {procedureName} {data1.ParamName}, {data2.ParamName}";
            var connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString);
            var command = new SqlCommand(sql, connection) { CommandTimeout = 120 };
            AddDataTable(data1, command);
            AddDataTable(data2, command);
            connection.Open();
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, token);
        }

        private static void AddDataTable<T>(DataToExecute<T> data, SqlCommand command)
        {
            DataTable table = data.Data.ToDataTable();
            foreach (var excludedColumn in data.ExcludedColumns)
                if (table.Columns.Contains(excludedColumn))
                    table.Columns.Remove(excludedColumn);

            SqlParameter parameter = new SqlParameter(data.ParamName, table)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = data.TypeName
            };

            command.Parameters.Add(parameter);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            DataTable table = new DataTable();

            //// get properties of T 
            var binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            var options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer | PropertyReflectionOptions.IgnoreClasses;

            var properties = ReflectionExtensions.GetProperties<T>(binding, options).ToList();

            //// create table schema based on properties 
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name,
                    property.PropertyType.Name == "Nullable`1"
                        ? property.PropertyType.GenericTypeArguments[0]
                        : (property.PropertyType.BaseType == typeof(Enum) ? typeof(int) : property.PropertyType));
            }

            //// create table data from T instances 
            object[] values = new object[properties.Count];

            foreach (T item in source)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }

            return table;
        }

        public static List<PropertyInfo> GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Length == 0).ToList();
        }

        public static async Task ExecuteStoredProcedureAsync(this DbContext context,
             string procedureName, CancellationToken token, params SqlParameter[] parameters)
        {
            var command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = procedureName,
                CommandTimeout = 120,
                Connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString),
            };
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            try
            {
                await command.ExecuteNonQueryAsync(token);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public static async Task<IDataReader> ExecuteStoredProcedureReaderAsync(this DbContext context,
            string procedureName, CancellationToken token, params SqlParameter[] parameters)
        {
            var command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = procedureName,
                CommandTimeout = 120,
                Connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString),
            };
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, token);
        }

        public static IDataReader ExecuteStoredProcedureReader(this DbContext context, string procedureName, params SqlParameter[] parameters)
        {
            var command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = procedureName,
                CommandTimeout = 120,
                Connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString),
            };
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }


    }

}

