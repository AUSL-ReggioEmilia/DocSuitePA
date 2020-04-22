using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class AdvancedProtocolModelMapper : BaseModelMapper<AdvancedProtocol, AdvancedProtocolModel>, IAdvancedProtocolModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public AdvancedProtocolModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]

        public override AdvancedProtocolModel Map(AdvancedProtocol entity, AdvancedProtocolModel modelTransformed)
        {
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number;
            modelTransformed.Note = entity.Note;
            modelTransformed.ServiceCategory = entity.ServiceCategory;
            modelTransformed.Subject = entity.Subject;
            modelTransformed.ServiceField = entity.ServiceField;
            modelTransformed.Note = entity.Note;
            modelTransformed.Origin = entity.Origin;
            modelTransformed.Package = entity.Package;
            modelTransformed.Lot = entity.Lot;
            modelTransformed.Incremental = entity.Incremental;
            modelTransformed.InvoiceNumber = entity.InvoiceNumber;
            modelTransformed.InvoiceDate = entity.InvoiceDate;
            modelTransformed.InvoiceTotal = entity.InvoiceTotal;
            modelTransformed.AccountingSectional = entity.AccountingSectional;
            modelTransformed.AccountingYear = entity.AccountingYear;
            modelTransformed.AccountingDate = entity.AccountingDate;
            modelTransformed.AccountingNumber = entity.AccountingNumber;
            modelTransformed.IsClaim = entity.IsClaim;
            modelTransformed.ProtocolStatus = entity.ProtocolStatus;
            modelTransformed.IdentificationSdi = entity.IdentificationSdi;
            modelTransformed.AccountingSectionalNumber = entity.AccountingSectionalNumber;
            modelTransformed.InvoiceYear = entity.InvoiceYear;


            return modelTransformed;
        }
        #endregion
    }
}
