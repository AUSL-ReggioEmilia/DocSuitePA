using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class AdvancedProtocolMapper : BaseEntityMapper<AdvancedProtocol, AdvancedProtocol>, IAdvancedProtocolMapper
    {
        public override AdvancedProtocol Map(AdvancedProtocol entity, AdvancedProtocol entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.ServiceCategory = entity.ServiceCategory;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
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

            return entityTransformed;
        }
    }
}
