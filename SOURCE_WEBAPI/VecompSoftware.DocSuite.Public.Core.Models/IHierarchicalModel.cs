using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models
{
    public interface IHierarchicalModel<TModel> : IModel
         where TModel : IModel
    {
        Guid? UniqueIdFather { get; set; }

        ICollection<TModel> Children { get; set; }
    }
}