using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.API.Connector.OChartService;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class OChartConnector : BaseConnector<OChartServiceClient>
    {

        #region [ Constructors ]

        public OChartConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.OChartService", Path.Combine(address, "OChartService.svc")) { }

        #endregion

        #region [ Methods ]

        public static OChartConnector For(string address)
        {
            return new OChartConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }
        public IOChartItemDTO[] GetEffective()
        {
            var json = Client.GetEffective().FromBase64();
            var effective = JsonConvert.DeserializeObject<OChartItemDTO[]>(json);
            SetParentRecursively(effective);
            return effective;
        }
        public bool Transform(IEnumerable<IOrgDeptDTO> dtos)
        {
            var json = JsonConvert.SerializeObject(dtos.ToArray()).ToBase64();
            return Client.Transform(json);
        }
        public bool Transform(IOrgDeptDTO dto)
        {
            var list = new List<IOrgDeptDTO> { dto };
            return Transform(list);
        }

        protected override void CreateClient()
        {
            Client = new OChartServiceClient(ConfigurationName, Address);
        }

        public void SetParentRecursively(IOChartItemDTO[] items)
        {
            if (items.IsNullOrEmpty())
                return;

            foreach (var parent in items)
            {
                if (parent.Items.IsNullOrEmpty())
                    continue;
                parent.Items.ToList().ForEach(c => c.Parent = parent);
                SetParentRecursively(parent.Items);
            }
        }

        #endregion

    }
}
