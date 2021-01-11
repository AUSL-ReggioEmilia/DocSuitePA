using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.DTO.WebAPI
{
    public class WebAPIDto<T>
    {
        public T Entity { get; set; }
        public TenantModel TenantModel { get; set; }
    }
}
