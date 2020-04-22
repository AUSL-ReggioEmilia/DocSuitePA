using System.Linq;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class OChartService : IOChartService
    {
        public bool IsAlive()
        {
            return true;
        }

        public string GetEffective()
        {
            var effective = FacadeFactory.Instance.OChartFacade.GetEffective();
            var dtos = effective.Items.Where(i => i.IsRoot).Distinct().Select(i => new OChartItemDTO(i)).ToArray();
            return JsonConvert.SerializeObject(dtos, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize }).ToBase64();
        }
        public bool Transform(string arrayOrgDeptDTO)
        {
            var dtos = JsonConvert.DeserializeObject<OrgDeptDTO[]>(arrayOrgDeptDTO.FromBase64());
            return FacadeFactory.Instance.OChartFacade.Transform(dtos) != null;
        }

    }
}
