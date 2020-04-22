using System;
using System.Linq.Expressions;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
{
    public class SortExpression<T>
    {
        public Expression<Func<T, object>> Expression { get; set; }
        public SortDirection Direction { get; set; }
    }
}
