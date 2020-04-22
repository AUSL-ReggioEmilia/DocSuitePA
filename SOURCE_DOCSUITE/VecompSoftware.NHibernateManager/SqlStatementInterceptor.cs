using System.Diagnostics;
using NHibernate;

namespace VecompSoftware.NHibernateManager
{
    /// <summary>
    /// Interceptor per loggare su Console le query generate da NHibernate
    /// </summary>
    public class SqlStatementInterceptor : EmptyInterceptor
    {
        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            Trace.WriteLine(sql.ToString());
            return sql;
        }
    }
}
