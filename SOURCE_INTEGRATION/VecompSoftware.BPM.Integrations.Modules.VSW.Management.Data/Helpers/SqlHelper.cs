using System;
using System.Data;
using System.Data.SqlClient;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data.Helpers
{
    public static class SqlHelper
    {
        public static SqlParameter CreateSqlParameter(string paramName, object paramValue, DbType parameterDbType)
        {
            return CreateSqlParameter(paramName, 0, paramValue, parameterDbType, ParameterDirection.Input);
        }
        public static SqlParameter CreateSqlParameter(string paramName, int size, object paramValue, DbType parameterDbType)
        {
            return CreateSqlParameter(paramName, size, paramValue, parameterDbType, ParameterDirection.Input);
        }
        public static SqlParameter CreateSqlParameter(string paramName, int size, object paramValue, DbType parameterDbType, ParameterDirection paramDirection)
        {
            return new SqlParameter
            {
                ParameterName = paramName,
                DbType = parameterDbType,
                Direction = paramDirection,
                Size = size,
                Value = GetValueOrDefault(paramValue, parameterDbType)
            };
        }

        private static object GetValueOrDefault(object paramValue, DbType parameterDbType)
        {
            object validatedObjectValue = paramValue == null || (parameterDbType == DbType.String && string.IsNullOrEmpty(paramValue.ToString())) ? DBNull.Value : paramValue;

            return validatedObjectValue;
        }
    }
}
