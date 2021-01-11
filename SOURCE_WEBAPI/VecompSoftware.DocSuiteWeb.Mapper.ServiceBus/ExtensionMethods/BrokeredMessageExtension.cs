using Microsoft.ServiceBus.Messaging;
using System;
using System.Linq.Expressions;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.ExtensionMethods
{
    public static class BrokeredMessageExtension
    {
        public static T GetPropertyValueOrDefault<T>(this BrokeredMessage message, Expression<Func<T>> selector, T defaultValue)
        {
            try
            {
                MemberExpression member;
                if (selector.Body is MemberExpression)
                {
                    member = (MemberExpression)selector.Body;
                }
                else
                {
                    Expression unaryExpression = ((UnaryExpression)selector.Body).Operand;
                    member = (MemberExpression)unaryExpression;
                }
                Expression expression = member.Expression;
                defaultValue = selector.Compile()();
            }
            catch (Exception)
            {
                return defaultValue;
            }

            return defaultValue;
        }
    }
}
