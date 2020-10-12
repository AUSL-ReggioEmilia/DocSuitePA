using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Metadata;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID
{
    internal class SPIDMetadataModelWrapper
    {
        #region [ Fields ]
        private readonly ICollection<MetadataValueModel> _spidMetadataModel;
        private string _tipologiaUtente = null;
        private string _tipologiaAccesso = null;
        private string _documenti = null;
        private string _motivazioni = null;
        private string _ritornoDocumentazione = null;
        private string _statoRichiesta = null;
        private readonly IDictionary<string, string> _tipologiaUtenteLabels = new Dictionary<string, string>()
        {
            { "0", "Interessato" },
            { "1", "Legittimato" },
            { "2", "Delegato" }
        };
        private readonly IDictionary<string, string> _tipologiaAccessoLabels = new Dictionary<string, string>()
        {
            { "0", "Visione" },
            { "1", "Copia semplice" },
            { "2", "Copia autenticata" }
        };
        private readonly IDictionary<string, string> _ritornoDocumentazioneLabels = new Dictionary<string, string>()
        {
            { "0", "PEC" },
            { "1", "Ufficio competente" },
            { "2", "Raccomandata" }
        };
        private readonly IDictionary<string, string> _statoRichiestaLabels = new Dictionary<string, string>()
        {
            { "0", "Richiesta" },
            { "1", "In valutazione" },
            { "3", "Diniego" },
            { "4", "Esibita" }
        };
        #endregion

        #region [ Const ]
        private const string TIPOLOGIA_UTENTE_KEY = "Tipologia utente";
        private const string TIPOLOGIA_ACCESSO_KEY = "Tipologia accesso";
        private const string DOCUMENTI_KEY = "Documenti";
        private const string MOTIVAZIONI_KEY = "Motivazioni";
        private const string RITORNO_DOCUMENTAZIONE_KEY = "Ritorno documentazione";
        private const string STATO_RICHIESTA_KEY = "Stato richiesta";
        #endregion

        #region [ Properties ]
        public string TipologiaUtente
        {
            get
            {
                if (_tipologiaUtente == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(TIPOLOGIA_UTENTE_KEY);
                    if (fieldModel == null)
                    {
                        fieldModel = new MetadataValueModel() { Value = "0" };
                    }
                    _tipologiaUtente = _tipologiaUtenteLabels[fieldModel.Value];
                }
                return _tipologiaUtente;
            }
        }

        public string TipologiaAccesso
        {
            get
            {
                if (_tipologiaAccesso == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(TIPOLOGIA_ACCESSO_KEY);
                    if (fieldModel == null)
                    {
                        fieldModel = new MetadataValueModel() { Value = "0" };
                    }
                    _tipologiaAccesso = _tipologiaAccessoLabels[fieldModel.Value];
                }
                return _tipologiaAccesso;
            }
        }

        public string Documenti
        {
            get
            {
                if (_documenti == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(DOCUMENTI_KEY);
                    _documenti = fieldModel?.Value;
                }
                return _documenti;
            }
        }

        public string Motivazioni
        {
            get
            {
                if (_motivazioni == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(MOTIVAZIONI_KEY);
                    _motivazioni = fieldModel?.Value;
                }
                return _motivazioni;
            }
        }

        public string RitornoDocumentazione
        {
            get
            {
                if (_ritornoDocumentazione == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(RITORNO_DOCUMENTAZIONE_KEY);
                    if (fieldModel == null)
                    {
                        fieldModel = new MetadataValueModel() { Value = "0" };
                    }
                    _ritornoDocumentazione = _ritornoDocumentazioneLabels[fieldModel.Value];
                }
                return _ritornoDocumentazione;
            }
        }

        public string StatoRichiesta
        {
            get
            {
                if (_statoRichiesta == null)
                {
                    MetadataValueModel fieldModel = GetTextFieldModel(STATO_RICHIESTA_KEY);
                    if (fieldModel == null)
                    {
                        fieldModel = new MetadataValueModel() { Value = "0" };
                    }
                    _statoRichiesta = _statoRichiestaLabels[fieldModel.Value];
                }
                return _statoRichiesta;
            }
        }
        #endregion

        #region [ Constructor ]
        public SPIDMetadataModelWrapper(ICollection<MetadataValueModel> spidMetadataModel)
        {
            _spidMetadataModel = spidMetadataModel;
        }
        #endregion

        #region [ Methods ]
        private MetadataValueModel GetTextFieldModel(string keyName)
        {
            return _spidMetadataModel.SingleOrDefault(x => x.KeyName.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}
