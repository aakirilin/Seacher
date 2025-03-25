using Npgsql;
using Seacher.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Common
{
    public delegate DbConnection CreateDbConnection();
    public delegate DbCommand CreateDbCommand();

    public class SQLAdapter : IDisposable, ISQLAdapter
    {
        public static SQLAdapter Create(DBMSTypes dbms, string cs)
        {
            if(dbms == DBMSTypes.SQLight)
            {
                return new SQLAdapter(
                    () => new Microsoft.Data.Sqlite.SqliteConnection(cs),
                    () => new Microsoft.Data.Sqlite.SqliteCommand()
                );
            }
            else if(dbms == DBMSTypes.MSSQL)
            {
                return new SQLAdapter(
                    () => new Microsoft.Data.SqlClient.SqlConnection(cs),
                    () => new Microsoft.Data.SqlClient.SqlCommand()
                );
            }
            else if (dbms == DBMSTypes.PostgreSQL)
            {
                return new SQLAdapter(
                    () => new NpgsqlConnection(cs),
                    () => new NpgsqlCommand()
                );
            }
            else if (dbms == DBMSTypes.Oracle)
            {
                return new SQLAdapter(
                    () => new OracleConnection(cs),
                    () => new OracleCommand()
                );
            }
            else if (dbms == DBMSTypes.MSSQL)
            {
                return new SQLAdapter(
                    () => new Microsoft.Data.SqlClient.SqlConnection(cs),
                    () => new Microsoft.Data.SqlClient.SqlCommand()
                );
            }
            else if (dbms == DBMSTypes.MariaDb)
            {
                return new SQLAdapter(
                    () => new Microsoft.Data.SqlClient.SqlConnection(cs),
                    () => new Microsoft.Data.SqlClient.SqlCommand()
                );
            }
            else
                throw new NotImplementedException();
        }

        private CreateDbConnection createDbConnection;
        private CreateDbCommand createDbCommand;

        public static string DBFileName = "settings.db";
        public static string SettingsCS => $"Data Source={DBFileName}";

        private DbConnection connection;

        public SQLAdapter(CreateDbConnection createDbConnection, CreateDbCommand createDbCommand)
        {
            this.createDbConnection = createDbConnection;
            this.createDbCommand = createDbCommand;

            connection = createDbConnection.Invoke();
        }

        public void Open()
        {
            connection.Open();
        }
        public void Close()
        {
            connection.Close();
        }
        public void ExecuteNonQuery(string qerrys, string delimiter = ";")
        {
            connection.Open();
            DbCommand command = null;
            try
            {
                command = createDbCommand();
                command.Transaction = connection.BeginTransaction();
                command.Connection = connection;
                foreach (var qerry in qerrys.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(qerry))
                    {
                        command.CommandText = qerry;
                        command.ExecuteNonQuery();
                    }
                }
                command?.Transaction?.Commit();
            }
            catch (Exception ex)
            {
                command?.Transaction?.Rollback();
                throw;
            }
            finally
            {
                connection?.Close();
                command?.Dispose();
            }
        }


        private object ReadColumnValue(Type resultType, DbDataReader reader, int columnIndex)
        {
            Debug.WriteLine(reader.GetValue(columnIndex));
            if (resultType == typeof(string) && reader.IsDBNull(columnIndex)) 
                return null;
            else if (resultType == typeof(string)) 
                return reader.GetString(columnIndex);
            else if (resultType == typeof(Int32) && !reader.IsDBNull(columnIndex))
                return reader.GetInt32(columnIndex);
            else if (resultType == typeof(Int16) && !reader.IsDBNull(columnIndex))
                return reader.GetInt16(columnIndex);
            else if (resultType == typeof(Int64) && !reader.IsDBNull(columnIndex))
                return reader.GetInt64(columnIndex);
            else if (resultType == typeof(Int32) && reader.IsDBNull(columnIndex))
                return 0;
            else if (resultType == typeof(Int16) && reader.IsDBNull(columnIndex))
                return 0;
            else if (resultType == typeof(Int64) && reader.IsDBNull(columnIndex))
                return 0;
            else throw new NotImplementedException();

        }
        public IEnumerable<object> ExecuteReader(Type type, ExecuteReaderParams @params)
        {
            connection.Open();

            List<object> result = new List<object>();

            var propertes = type.GetProperties()
                .OrderBy(p => p.Name)
                .ToList();

            if (String.IsNullOrWhiteSpace(@params.Condition)) @params.Condition = "1 = 1";

            var qerry = $"""
                select {String.Join(", ", @params.Fields.OrderBy(f => f.Name).Select(f => f.Name))} 
                from {String.Join(" ", @params.Tablets)} 
                where {@params.Condition}
                """;

            DbCommand command = null;
            try
            {
                command = createDbCommand();
                command.Transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.CommandText = qerry;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            object t = Activator.CreateInstance(type);
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var property = propertes[i];
                                property.SetValue(t, ReadColumnValue(property.PropertyType, reader, i));
                            }
                            result.Add(t);
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                throw;
            }
            finally
            {
                command?.Transaction?.Rollback();
                connection?.Close();
                command?.Dispose();
            }

            return result;
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
