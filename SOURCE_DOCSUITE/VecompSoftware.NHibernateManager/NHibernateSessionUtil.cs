using NHibernate;
using System;
using System.Collections.Generic;

namespace VecompSoftware.NHibernateManager
{
    public class NHibernateSessionUtil
    {
        public static ICollection<Action<ISession>> ApplyFilterActions = new List<Action<ISession>>();
    }
}
