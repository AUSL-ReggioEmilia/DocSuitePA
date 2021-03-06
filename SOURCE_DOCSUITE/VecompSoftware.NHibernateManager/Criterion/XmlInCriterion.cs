﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml;
using global::NHibernate;
using global::NHibernate.Criterion;
using global::NHibernate.Dialect;
using global::NHibernate.Engine;
using global::NHibernate.Persister.Entity;
using global::NHibernate.SqlCommand;
using global::NHibernate.SqlTypes;
using global::NHibernate.Type;
using global::NHibernate.UserTypes;
using nhutil = global::NHibernate.Util;

namespace VecompSoftware.NHibernateManager.Criterion
{
    public class XmlInCriterion : AbstractCriterion
    {
        private readonly AbstractCriterion expr;
        private readonly string propertyName;
        private readonly object[] values;
        private readonly int maximumNumberOfParametersToNotUseXml = 2000;

        public static AbstractCriterion Create(string property, IEnumerable values)
        {
            return new XmlInCriterion(property, values);
        }

        /// <summary>
        /// Creates the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="values">The values.</param>
        /// <param name="maximumNumberOfParametersToNotUseXml">The maximum number of paramters allowed before the XmlIn creates an xml string.</param>
        /// <returns></returns>
        public static AbstractCriterion Create(string property, IEnumerable values, int maximumNumberOfParametersToNotUseXml)
        {
            return new XmlInCriterion(property, values, maximumNumberOfParametersToNotUseXml);
        }

        public XmlInCriterion(string propertyName, IEnumerable values, int maximumNumberOfParametersToNotUseXml)
            : this(propertyName, values)
        {
            this.maximumNumberOfParametersToNotUseXml = maximumNumberOfParametersToNotUseXml;
        }

        public XmlInCriterion(string propertyName, IEnumerable values)
        {
            this.propertyName = propertyName;
            ArrayList arrayList = new ArrayList();
            foreach (object o in values)
            {
                arrayList.Add(o);
            }
            this.values = arrayList.ToArray();
            expr = Expression.In(propertyName, arrayList);
        }

        public override string ToString()
        {
            return propertyName + " big in (" + nhutil.StringHelper.ToString(values) + ')';
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            //we only need this for SQL Server, and or large amount of values
            if ((criteriaQuery.Factory.Dialect is MsSql2005Dialect) == false || values.Length < maximumNumberOfParametersToNotUseXml)
            {
                return expr.ToSqlString(criteria, criteriaQuery);
            }
            //TODO: NON FUNZIONA DOPO IL REFACTOR NHIBERNATE 5
            IType type = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);
            if (type.IsCollectionType)
            {
                throw new QueryException("Cannot use collections with InExpression");
            }

            if (values.Length == 0)
            {
                // "something in ()" is always false
                return new SqlString("1=0");
            }

            SqlStringBuilder result = new SqlStringBuilder();
            string[] columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);

            // Generate SqlString of the form:
            // columnName1 in (xml query) and columnName2 in (xml query) and ...
            IEnumerable<Parameter> parameters = criteriaQuery.NewQueryParameter(this.GetTypedValues(criteria, criteriaQuery).First<TypedValue>());
                                    
            for (int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
            {
                string columnName = columnNames[columnIndex];

                if (columnIndex > 0)
                {
                    result.Add(" and ");
                }
                SqlType sqlType = type.SqlTypes(criteriaQuery.Factory)[columnIndex];
                result
                        .Add(columnName)
                        .Add(" in (")
                        .Add("SELECT ParamValues.Val.value('.','")
                        .Add(criteriaQuery.Factory.Dialect.GetTypeName(sqlType))
                        .Add("') FROM ")
                        .Add(parameters.ElementAt(columnIndex))
                        .Add(".nodes('/items/val') as ParamValues(Val)")
                        .Add(")");
            }

            return result.ToSqlString();
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            //we only need this for SQL Server, and or large amount of values
            if ((criteriaQuery.Factory.Dialect is MsSql2005Dialect) == false || values.Length < maximumNumberOfParametersToNotUseXml)
            {
                return expr.GetTypedValues(criteria, criteriaQuery);
            }

            IEntityPersister persister = null;
            IType type = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);

            if (type.IsEntityType)
            {
                persister = criteriaQuery.Factory.GetEntityPersister(type.ReturnedClass.FullName);
            }
            StringWriter sw = new StringWriter();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = System.Text.Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(sw, xmlWriterSettings);
            writer.WriteStartElement("items");
            foreach (object value in values)
            {
                if (value == null)
                    continue;
                object valToWrite;
                if (persister != null)
                    valToWrite = persister.GetIdentifier(value);
                else
                    valToWrite = value;
                writer.WriteElementString("val", valToWrite.ToString());
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            string xmlString = sw.GetStringBuilder().ToString();

            return new TypedValue[] { 
                                new TypedValue(new CustomType(typeof(XmlType), 
                                new Dictionary<string, string>()), xmlString), };
        }

        private class XmlType : IUserType
        {
            private static readonly SqlType[] sqlTypes = new SqlType[] { new SqlType(DbType.Xml) };
            private readonly Type returnedType = typeof(string);
            private readonly bool isMutable = false;

            public SqlType[] SqlTypes
            {
                get { return sqlTypes; }
            }

            public Type ReturnedType
            {
                get { return returnedType; }
            }

            public new bool Equals(object x, object y)
            {
                return Object.Equals(x, y);
            }

            public int GetHashCode(object x)
            {
                return x.GetHashCode();
            }

            public object NullSafeGet(IDataReader rs, string[] names, object owner)
            {
                return null;
            }

            public void NullSafeSet(IDbCommand cmd, object value, int index)
            {
                IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
                parameter.Value = value;
            }

            public object DeepCopy(object value)
            {
                return value;
            }

            public bool IsMutable
            {
                get { return isMutable; }
            }

            public object Replace(object original, object target, object owner)
            {
                return original;
            }

            public object Assemble(object cached, object owner)
            {
                return cached;
            }

            public object Disassemble(object value)
            {
                return value;
            }

            public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
            {
                return null;
            }

            public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
            {
                (cmd.Parameters[index]).Value = DBNull.Value ;
            }
        }

        public override IProjection[] GetProjections()
        {
            return null;
        }
    }
}
