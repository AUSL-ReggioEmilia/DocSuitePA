using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Configuration;
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

        public bool ValidateTenantNameInvoice()
        {
            /*
             * _logger.WriteWarning(new LogMessage($"Invalid company name {fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items} : {currentTenant.CompanyName}. Workflow will be skipped"), LogCategories);
             * throw new ArgumentException($"Invalid company name {fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items} : {currentTenant.CompanyName}. Workflow will be skipped");
             */
            return
                    _fatturaElettronica.FatturaElettronicaHeader != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica != null &&
                    _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items != null
                    ? string.Join(" ", _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items).Equals(_currentTenant.CompanyName)
                    : false;
        }

        public bool ValidateTenantPiva(out string tenantPiva)
        {
            tenantPiva = _currentTenant.TenantWorkflowRepositories.SingleOrDefault(f => f.IntegrationModuleName == ModuleConfigurationHelper.MODULE_NAME)?.JsonValue;
            if (!string.IsNullOrEmpty(tenantPiva))
            {
                if (_fatturaElettronica.FatturaElettronicaHeader != null &&
                        _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente != null &&
                        _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici != null)
                {
                    string invoicePiva = _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.CodiceFiscale;
                    IdFiscaleType idFiscaleType;
                    if ((idFiscaleType = _fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA) != null)
                    {
                        invoicePiva = $"{idFiscaleType.IdPaese}{idFiscaleType.IdCodice}";
                    }
                    return invoicePiva.Equals(tenantPiva, System.StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return true;
        }
    }
}
