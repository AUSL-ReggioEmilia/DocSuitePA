using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class AdvacedProtocolTableValuedModelMapper : BaseModelMapper<AdvancedProtocolTableValuedModel, AdvancedProtocolModel>, IAdvancedProtocolTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public AdvacedProtocolTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override AdvancedProtocolModel Map(AdvancedProtocolTableValuedModel model, AdvancedProtocolModel modelTransformed)
        {
            modelTransformed.Year = model.Year;
            modelTransformed.Number = model.Number;
            modelTransformed.ServiceCategory = model.ServiceCategory;
            modelTransformed.Subject = model.Subject;
            modelTransformed.ServiceField = model.ServiceField;
            modelTransformed.Note = model.Note;
            modelTransformed.Origin = model.Origin;
            modelTransformed.Package = model.Package;
            modelTransformed.Lot = model.Lot;
            modelTransformed.Incremental = model.Incremental;
            modelTransformed.InvoiceNumber = model.InvoiceNumber;
            modelTransformed.InvoiceDate = model.InvoiceDate;
            modelTransformed.InvoiceTotal = model.InvoiceTotal;
            modelTransformed.AccountingYear = model.AccountingYear;
            modelTransformed.AccountingDate = model.AccountingDate;
            modelTransformed.AccountingNumber = model.AccountingNumber;
            modelTransformed.IsClaim = model.IsClaim;
            modelTransformed.ProtocolStatus = model.ProtocolStatus;
            modelTransformed.IdentificationSdi = model.IdentificationSdi;
            modelTransformed.AccountingSectionalNumber = model.AccountingSectionalNumber;
            modelTransformed.InvoiceYear = model.InvoiceYear;

            return modelTransformed;
        }
        public override ICollection<AdvancedProtocolModel> MapCollection(ICollection<AdvancedProtocolTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<AdvancedProtocolModel>();
            }
            List<AdvancedProtocolModel> modelsTransformed = new List<AdvancedProtocolModel>();
            AdvancedProtocolModel modelTransformed = null;
            foreach (IGrouping<int, AdvancedProtocolTableValuedModel> advancedProtocolLookup in model.ToLookup(x => x.Number))
            {
                modelTransformed = Map(advancedProtocolLookup.First(), new AdvancedProtocolModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
