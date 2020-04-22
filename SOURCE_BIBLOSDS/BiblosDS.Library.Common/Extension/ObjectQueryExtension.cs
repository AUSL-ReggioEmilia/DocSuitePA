using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using BiblosDS.Library.Common.Extension;

namespace System.Data
{
    public static class ObjectQueryExtension
    {
        /// <summary>
        /// Specifies the related objects to include in the query results using
        /// a lambda expression listing the path members.
        /// </summary>
        /// <returns>A new System.Data.Objects.ObjectQuery&lt;T&gt; with the defined query path.</returns>
        public static ObjectQuery<T> Include<T>(this ObjectQuery<T> query, Expression<Func<T, object>> path)
        {
            // Retrieve member path:
            List<ExtendedPropertyInfo> members = new List<ExtendedPropertyInfo>();
            EntityFrameworkHelper.CollectRelationalMembers(path, members);

            // Build string path:
            StringBuilder sb = new StringBuilder();
            string separator = "";
            foreach (ExtendedPropertyInfo member in members)
            {
                sb.Append(separator);
                sb.Append(member.PropertyInfo.Name);
                separator = ".";
            }

            // Apply Include:
            return query.Include(sb.ToString());
        }

        public static IQueryable<T> WhereOr<T>(this IEnumerable<T> Source, List<Expression<Func<T, bool>>> Predicates)
        {

            Expression<Func<T, bool>> FinalQuery;

            FinalQuery = e => false;

            foreach (Expression<Func<T, bool>> Predicate in Predicates)
            {
                FinalQuery = FinalQuery.Or(Predicate);
            }

            return Source.AsQueryable<T>().Where(FinalQuery);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> Source, Expression<Func<T, bool>> Predicate)
        {
            InvocationExpression invokedExpression = Expression.Invoke(Predicate, Source.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.Or(Source.Body, invokedExpression), Source.Parameters);
        } 

    }
}
