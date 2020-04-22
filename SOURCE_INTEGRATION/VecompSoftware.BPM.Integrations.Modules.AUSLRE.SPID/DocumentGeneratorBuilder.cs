using System;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID
{
    internal class DocumentGeneratorBuilder
    {
        #region [ Fields ]
        private readonly SPIDMetadataModelWrapper _spidRequestModel;
        private readonly ContactModel _contact;
        #endregion

        #region [ Constructor ]
        public DocumentGeneratorBuilder(SPIDMetadataModelWrapper model, ContactModel contact)
        {
            _spidRequestModel = model;
            _contact = contact;
        }
        #endregion

        #region [ Methods ]
        public DocumentGeneratorModel Build()
        {
            DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel();
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("nome_cognome", _contact.Description.Replace('|', ' ')));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("nato_a", _contact.BirthPlace));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("nato_il", _contact.BirthDate?.ToString("dd/MM/yyyy")));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("residente_in", _contact.City));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("cap", _contact.ZipCode));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("indirizzo", _contact.Address));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("numero_civico", _contact.CivicNumber));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("telefono", _contact.TelephoneNumber));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("in_qualita_di", _spidRequestModel.TipologiaUtente));
            string chkVisioneVal = _spidRequestModel.TipologiaAccesso.Equals("Visione", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("visione", chkVisioneVal));
            string chkCopiaSempliceVal = _spidRequestModel.TipologiaAccesso.Equals("Copia semplice", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("copia_semplice", chkCopiaSempliceVal));
            string chkCopiaAutenticataVal = _spidRequestModel.TipologiaAccesso.Equals("Copia autenticata", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("copia_autenticata", chkCopiaAutenticataVal));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("documenti", _spidRequestModel.Documenti));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("motivazioni", _spidRequestModel.Motivazioni));
            string chkRitiroPECVal = _spidRequestModel.RitornoDocumentazione.Equals("PEC", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("ritiro_pec", chkRitiroPECVal));
            string chkRitiroUfficioVal = _spidRequestModel.RitornoDocumentazione.Equals("Ufficio competente", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("ritiro_ufficio", chkRitiroUfficioVal));
            string chkRititoRaccomandataVal = _spidRequestModel.RitornoDocumentazione.Equals("Raccomandata", StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "Off";
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("ritiro_raccomandata", chkRititoRaccomandataVal));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("indirizzo_ricevuta", "."));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("data_esecuzione", DateTime.Today.ToString("dd/MM/yyyy")));
            documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter("richiedente", _contact.Description.Replace('|', ' ')));
            return documentGeneratorModel;
        }
        #endregion
    }
}
