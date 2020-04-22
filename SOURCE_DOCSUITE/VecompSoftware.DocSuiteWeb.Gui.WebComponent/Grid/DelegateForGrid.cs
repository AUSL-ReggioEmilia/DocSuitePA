using System.Collections;
using Telerik.Web.UI;

namespace VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
{
    public class DelegateForGrid<T, THeader>
        where T : class
        where THeader : class
    {
        /// <summary>
        /// Vengono registrati i delegati per la BindGrid:
        /// 1) DoSearchHeader
        /// 2) DataBindingFinder
        /// 3) CreateSortingExpression
        /// 
        /// I delegati sono necessari per utilizzare la BindGrid e devono essere implementati!
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static BindGrid Delegate(BindGrid grid)
        {
            if ((grid.DelegateDoSearchHeader == null))
            {
                grid.DelegateDoSearchHeader = (() => (ICollection)grid.FinderExecuteDoSearchHeader<T, THeader>());
            }
            
            if ((grid.DelegateDataBindingFinder == null))
            {
                grid.DelegateDataBindingFinder = (() => grid.DataBindFinder<T, THeader>());
            }

            if ((grid.DelegateCreateSortingExpression == null))
            {
                grid.DelegateCreateSortingExpression = ((GridSortCommandEventArgs e) => grid.FinderSorting<T, THeader>(e));
            }
            return grid;
        }
    }
}
