using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.Helpers.XML.Converters.Models.InvoicePA.PA1_2;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice
{
    public class InvoiceValidation
    {
        private readonly FatturaElettronicaType _fatturaElettronica;
        private readonly Tenant _currentTenant;

        public InvoiceValidation(FatturaElettronicaType fatturaElettronica, Tenant currentTenant)
        {
            _fatturaElettronica = fatturaElettronica;
            _currentTenant = currentTenant;
        }

        public bool ValidateInvoice()
        {
            return
                    _fatturaElettronica.FatturaElettronicaHeader != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items != null
                    ? string.Join(" ", _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items).Equals(_currentTenant.CompanyName)
                    : false;
        }
    }
}
