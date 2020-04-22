using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuite.Public.Core.Models.Helpers.ExtensionMethods
{
    public static class BuildHierarchicalModel
    {
        //IHierarchicalModel
        public static ICollection<TModel> BuildHierarchical<TModel>(this List<TModel> items)
            where TModel : IHierarchicalModel<TModel>
        {

            items.ForEach(item => item.Children = items
                .Where(child => child.UniqueIdFather.HasValue && child.UniqueIdFather.Value == item.Id)
                .ToList());
            return items.Where(item => !item.UniqueIdFather.HasValue).ToList();
        }
    }
}
