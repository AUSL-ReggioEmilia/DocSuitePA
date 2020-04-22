namespace VecompSoftware.DocSuiteWeb.API
{
    public class TableDocTypeDTO : ITableDocTypeDTO
    {
        #region [ Constructor ]
        public TableDocTypeDTO()
        {

        }
        #endregion

        #region [ Properties ]        
        public string Code { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        #endregion
    }
}
