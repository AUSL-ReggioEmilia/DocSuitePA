using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;

namespace VecompSoftware.NHibernateManager.Transformer
{
    public class TopRecordsResultTransformer : IResultTransformer
    {
        private int topCount;

	    private static readonly IdentityEqualityComparer IdentityHashCodeProvider = new IdentityEqualityComparer();
        public TopRecordsResultTransformer(int topCount)
	    {
		    this.topCount = topCount;
	    }

	    public IList TransformList(IList collection)
	    {
		    IList list2 = new ArrayList();
		    ISet<Identity> @set = new HashSet<Identity>();
		    int i = 0;
		    for (i = 0; i <= collection.Count - 1; i++) 
            {
			    object entity = collection[i];
			    if (@set.Add(new Identity(entity))) 
                {
				    list2.Add(entity);
			    }
		    }

		    var list = new ArrayList();
		    int count = Math.Min(list2.Count, topCount);
		    for (i = 0; i <= count - 1; i++) 
            {
			    list.Add(list2[i]);
		    }
		    return list;
	    }

	    public object TransformTuple(object[] tuple, string[] aliases)
	    {
		    return tuple[tuple.Length - 1];
	    }


	    #region "Nested Class Identity"
	    internal sealed class Identity
	    {

		    internal readonly object entity;
		    internal Identity(object entity)
		    {
			    this.entity = entity;
		    }

		    public override bool Equals(object other)
		    {
			    Identity identity = (Identity)other;
			    return (object.ReferenceEquals(this.entity, identity.entity));
		    }

		    public override int GetHashCode()
		    {
			    return TopRecordsResultTransformer.IdentityHashCodeProvider.GetHashCode(this.entity);
		    }
	    }
	    #endregion
        }
}
