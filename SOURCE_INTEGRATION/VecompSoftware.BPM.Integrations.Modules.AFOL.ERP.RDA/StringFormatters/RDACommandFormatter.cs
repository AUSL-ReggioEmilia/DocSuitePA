using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.StringFormatters
{
    public class RDACommandFormatter : IFormatProvider, ICustomFormatter
    {
        #region [ Fields ]
        private IDictionary<string, Func<RDADocSuiteCommand, string>> _formatFragmentsValues;
        #endregion

        #region [ Constants ]
        private const string NUMBER = "Number";
        private const string SUPPLIER_NAME = "SupplierName";
        private const string PIVACF = "PIVACF";
        private const string COST_CENTER = "CostCenter";
        private const string TYPOLOGY = "Typology";
        private const string APPLICANT_AREA = "ApplicantArea";
        #endregion

        #region [ Constructor ]
        public RDACommandFormatter()
        {
            this._formatFragmentsValues = new Dictionary<string, Func<RDADocSuiteCommand, string>>(StringComparer.InvariantCultureIgnoreCase);
            this._formatFragmentsValues.Add(NUMBER, (RDADocSuiteCommand command) => command.Number);
            this._formatFragmentsValues.Add(SUPPLIER_NAME, (RDADocSuiteCommand command) => command.SupplierName);
            this._formatFragmentsValues.Add(PIVACF, (RDADocSuiteCommand command) => command.SupplierPIVACF);
            this._formatFragmentsValues.Add(COST_CENTER, (RDADocSuiteCommand command) => command.CostCenter);
            this._formatFragmentsValues.Add(TYPOLOGY, (RDADocSuiteCommand command) => command.Typology);
            this._formatFragmentsValues.Add(APPLICANT_AREA, (RDADocSuiteCommand command) => command.ApplicantArea);
        }
        #endregion

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string[] formatFragments = format.Split(':');
            RDADocSuiteCommand command = arg as RDADocSuiteCommand;

            if(command == null)
            {
                throw new ArgumentNullException("RDADocSuiteCommand cannot be null");
            }

            string formatKey = formatFragments[0];
            if (!_formatFragmentsValues.ContainsKey(formatKey))
            {
                throw new ArgumentException($"Invalid property name {formatKey}");
            }

            return _formatFragmentsValues[formatKey].Invoke(command);
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
    }
}
