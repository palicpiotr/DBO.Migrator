using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using NLog;
using System.Threading;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using DBO.DataTransport.HelpersStandard;
using DBO.DataTransport.HelpersStandard.Reflection;

namespace DBO.DataTransport.HelpersStandard.Helpers
{
    public static class EntityFrameworkHelper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static List<T> ToListReadUncommitted<T>(this IQueryable<T> query)
        {
            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }))
            {
                List<T> toReturn = query.ToList();
                scope.Complete();
                return toReturn;
            }
        }
        /// <summary> 
        /// Execute stored procedure with single table value parameter. 
        /// </summary> 
        /// <typeparam name="T">Type of object to store.</typeparam> 
        /// <param name="context">DbContext instance.</param> 
        /// <param name="data">Data to store</param> 
        /// <param name="procedureName">Procedure name</param> 
        /// <param name="paramName">Parameter name</param> 
        /// <param name="typeName">User table type name</param>
        /// <param name="excludedColumns">Columns to exclude</param> 
        public static SqlDataReader ExecuteTableValueProcedure<T>(this DbContext context, IEnumerable<T> data, string procedureName, string paramName, string typeName)
        {
            //// convert source data to DataTable 
            DataTable table = data.ToDataTable();
            return context.ExecuteTableValueProcedure(table, procedureName, paramName, typeName);
        }

        public static SqlDataReader ExecuteTableValueProcedure(this DbContext context, DataTable data, string procedureName,
            string paramName, string typeName)
        {
            var names = context.GetTypeNames(typeName, CancellationToken.None).Result;

            foreach (var excludedColumn in CheckColumns(data, names))
                data.Columns.Remove(excludedColumn);

            //// create parameter 
            SqlParameter parameter = new SqlParameter(paramName, data)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = typeName
            };

            //// execute sp sql 
            string sql = $"EXEC {procedureName} {paramName};";
            var connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString);
            var command = new SqlCommand(sql, connection) { CommandTimeout = 120 };
            command.Parameters.Add(parameter);
            connection.Open();
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            //// execute sql 
            return reader;
        }

        public static int SaveChangesWithLog(this DbContext context, ILogger log)
        {
            //context.ChangeTracker.DetectChanges();
            //var modifiedEntities = context.ChangeTracker.Entries()
            //    .Where(p => p.State == EntityState.Modified ||
            //    p.State == EntityState.Added ||
            //    p.State == EntityState.Deleted ||
            //    p.State == EntityState.Modified ||
            //    p.State == EntityState.Detached)
            //    .Select(x => x.Entity)
            //    .Where(x => x != ValidationResult.Success);

            //try
            //{
            return context.SaveChanges();
            //}
            //catch (DbEntityValidationResult e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        log.Error("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            log.Error("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }

        private static readonly Dictionary<string, HashSet<string>> _tableTypesColumns = new Dictionary<string, HashSet<string>>();

        private static async Task<HashSet<string>> GetTypeNames(this DbContext context, string typeName, CancellationToken token)
        {
            if (_tableTypesColumns.TryGetValue(typeName, out var names))
                return names;

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

            return _tableTypesColumns[typeName] = names;
        }

        private static List<string> CheckColumns(DataTable data, HashSet<string> names)
        {
            var res = new List<string>();
            foreach (DataColumn column in data.Columns)
                if (!names.Contains(column.ColumnName.ToLower()))
                    res.Add(column.ColumnName);
            return res;
        }

        /// <summary> 
        /// Creates data table from source data. 
        /// </summary> 
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

        public static async Task AsTransaction(this DbContext dc, Func<Task> transaction)
        {
            using (var scope = dc.Database.BeginTransaction())
            {
                try
                {
                    await transaction();
                    scope.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    scope.Rollback();
                }
            }
        }
    }
}
