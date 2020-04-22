using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Linq.Expressions;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "BiblosDSObject", Namespace = "http://BiblosDS/2009/10/BiblosDSObject")]    
    public class BiblosDSObject
    {
        private bool loaded = true;

        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; }
        }

        internal List<string> ModifiedField { get; set;}

        public BiblosDSObject()
        {
            loaded = false;
            ModifiedField = new List<string>();
        }

        public void ClearModifiedField()
        {
            ModifiedField.Clear();
        }

        public bool IsOnChangeList<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return ModifiedField.Contains(memberExpression.Member.Name);
        }
    }
}
