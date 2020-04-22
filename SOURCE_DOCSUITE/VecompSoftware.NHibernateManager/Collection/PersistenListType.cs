using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace VecompSoftware.NHibernateManager.Collection
{
    public class PersistentListType<TypeOfCollectionItem, TypeOfDataCollection> : IUserCollectionType where TypeOfDataCollection : PersistentGenericBag<TypeOfCollectionItem>, IList<TypeOfCollectionItem>
    {

        #region "IUserCollectionType Members"

        public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            Type dataCollectionType = typeof(TypeOfDataCollection);
            ConstructorInfo dataCollectionConstructor = dataCollectionType.GetConstructor(new Type[] { typeof(ISessionImplementor) });

            if (dataCollectionConstructor == null)
            {
                throw new DataException("dataCollectionConstructor did not have a constructor matching { ISessionImplementor }");
            }

            return (IPersistentCollection)dataCollectionConstructor.Invoke(new object[] { session });
        }

        public IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            Type dataCollectionType = typeof(TypeOfDataCollection);
            ConstructorInfo dataCollectionConstructor = dataCollectionType.GetConstructor(new Type[] {
			typeof(ISessionImplementor),
			typeof(IList<TypeOfCollectionItem>)
		});

            if (dataCollectionConstructor == null)
            {
                throw new DataException("dataCollectionConstructor did not have a constructor matching { ISessionImplementor, IList<" + typeof(TypeOfCollectionItem).ToString() + "> }");
            }

            return (IPersistentCollection)dataCollectionConstructor.Invoke(new object[] {
			session,
			collection
		});
        }

        public object Instantiate(int anticipatedSize)
        {
            return new List<TypeOfCollectionItem>();
        }

        public IEnumerable GetElements(object collection)
        {
            return (IEnumerable)collection;
        }

        public bool Contains(object collection, object entity)
        {
            return ((IList)collection).Contains(entity);
        }

        public object IndexOf(object collection, object entity)
        {
            return ((IList)collection).IndexOf(entity);
        }

        public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
        {
            var result = (IList)target;

            result.Clear();

            foreach (var o in (IEnumerable)original)
            {
                result.Add(o);
            }

            return result;
        }

        #endregion

    }
}
