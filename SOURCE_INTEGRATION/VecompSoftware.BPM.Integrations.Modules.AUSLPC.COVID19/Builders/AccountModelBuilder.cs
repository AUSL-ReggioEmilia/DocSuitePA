using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Builders
{
    public class AccountModelBuilder
    {
        #region [ Fields ]
        private readonly IDictionary<Func<DocumentUnit, string, IBuilder>, Func<DocumentUnit, bool>> _builderRulesValidations;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public AccountModelBuilder(IWebAPIClient webAPIClient)
        {
            _builderRulesValidations = new Dictionary<Func<DocumentUnit, string, IBuilder>, Func<DocumentUnit, bool>>()
            {
                { (du, fc) => new ProtocolAccountModelBuilder(webAPIClient, du, fc),
                    (du) => du.Environment == (int)DSWEnvironmentType.Protocol },
                { (du, fc) => new ArchiveAccountModelBuilder(webAPIClient, du, fc),
                    (du) => du.Environment >= 100 },
            };
        }
        #endregion

        #region [ Methods ]
        private IBuilder GetBuilder(DocumentUnit documentUnit, string fiscalCode)
        {
            return _builderRulesValidations.Where(x => x.Value(documentUnit) == true)
                .Select(s => s.Key(documentUnit, fiscalCode)).FirstOrDefault();
        }

        public AccountModel CreateAccountModel(DocumentUnit documentUnit, string fiscalCode)
        {
            IBuilder builder = GetBuilder(documentUnit, fiscalCode);
            if (builder == null)
            {
                throw new Exception($"Builder for DocumentUnit {documentUnit.UniqueId} not found");
            }

            return builder.Build();
        }
        #endregion
    }
}
