using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace VecompSoftware.JeepService
{
    internal class DatabaseHelper
    {

        #region [ Constructors ]

        public DatabaseHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region [ Properties ]

        public string ConnectionString { get; private set; }

        #endregion

        #region [ Methods ]

        public DataTable GetTable(string selectCommand, IEnumerable<SqlParameter> parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var adapter = new SqlDataAdapter(selectCommand, connection))
            {
                foreach (var item in parameters)
                    adapter.SelectCommand.Parameters.Add(item);
                
                adapter.SelectCommand.Connection.Open();
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
        public DataTable GetTable(string selectCommand, SqlParameter parameter)
        {
            return GetTable(selectCommand, new List<SqlParameter> { parameter });
        }

        public int ExecuteNonQuery(string nonQuery, IEnumerable<SqlParameter> parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(nonQuery, connection))
            {
                foreach (var item in parameters)
                    command.Parameters.Add(item);

                command.Connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        public int ExecuteNonQuery(string nonQuery, SqlParameter parameter)
        {
            return ExecuteNonQuery(nonQuery, new List<SqlParameter> { parameter });
        }

        #endregion

    }
}
