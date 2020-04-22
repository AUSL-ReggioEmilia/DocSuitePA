using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    /// <summary>
    /// 
    /// </summary>
    public class CategoryModel : DomainModel, IActiveModel, IHistoricizedModel
    {

        #region [ Fields ]
        private readonly string _hierarchyDescription = string.Empty;
        private readonly string _hierarchyCode = string.Empty;
        #endregion

        #region [ Contructors ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public CategoryModel(Guid id, string name) : base(id)
        {
            Name = name;
            Children = new HashSet<CategoryModel>();
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// 
        /// </summary>
        public string ArchiveSection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HierarchyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HierarchyDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ushort Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset StartDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CategoryModel Parent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<CategoryModel> Children { get; set; }

        #endregion
    }
}
