using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data
{
    public class SQLDbManager : ISQLDbManager
    {
        private readonly string _dbConnectionString;
        public SQLDbManager(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        /// <summary>
        ///     Executes the given sql query and returns the number of affected rows
        /// </summary>
        /// <param name="commandText">The raw SQL query</param>
        /// <param name="commandType">The type of the command: Text/StoredProcedure</param>
        /// <param name="parameters">SQL query parameters</param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlCommandAsync(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            using (SqlConnection dbConnection = new SqlConnection(_dbConnectionString))
            {
                await dbConnection.OpenAsync();
                using (SqlCommand sqlCommand = new SqlCommand(commandText, dbConnection))
                {
                    sqlCommand.Parameters.Clear();
                    sqlCommand.CommandType = commandType;

                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }

                    return await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
