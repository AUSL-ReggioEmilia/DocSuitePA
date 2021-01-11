using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class AdvancedProtocolValidatorMapper : BaseMapper<AdvancedProtocol, AdvancedProtocolValidator>, IAdvancedProtocolValidatorMapper
    {
        public AdvancedProtocolValidatorMapper() { }

        public override AdvancedProtocolValidator Map(AdvancedProtocol entity, AdvancedProtocolValidator entityTransformed)
        {
            #region [Base]

            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.ServiceCategory = entity.ServiceCategory;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.ServiceField = entity.ServiceField;
            entityTransformed.Note = entity.Note;
            entityTransformed.Origin = entity.Origin;
            entityTransformed.Package = entity.Package;
            entityTransformed.Lot = entity.Lot;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.InvoiceNumber = entity.InvoiceNumber;
            entityTransformed.InvoiceDate = entity.InvoiceDate;
            entityTransformed.InvoiceTotal = entity.InvoiceTotal;
            entityTransformed.AccountingSectional = entity.AccountingSectional;
            entityTransformed.AccountingYear = entity.AccountingYear;
            entityTransformed.AccountingDate = entity.AccountingDate;
            entityTransformed.AccountingNumber = entity.AccountingNumber;
            entityTransformed.IsClaim = entity.IsClaim;
            entityTransformed.ProtocolStatus = entity.ProtocolStatus;
            entityTransformed.IdentificationSdi = entity.IdentificationSdi;
            entityTransformed.AccountingSectionalNumber = entity.AccountingSectionalNumber;
            entityTransformed.InvoiceYear = entity.InvoiceYear;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;

        }

    }
}
