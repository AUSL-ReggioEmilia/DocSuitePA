
namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public interface IWebApiControllerEndpoint
    {
        string EndpointName { get; set; }

        string AddressName { get; set; }

        string ControllerName { get; set; }
    }
}
