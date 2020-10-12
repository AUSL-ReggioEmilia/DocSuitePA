using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.StringFormatters
{
    public class SkyDocCommandFormatter : IFormatProvider, ICustomFormatter
    {
        #region [ Fields ]
        private IDictionary<string, Func<SkyDocCommand, string>> _formatFragmentsValues;
        #endregion

        #region [ Constants ]
        private const string DOSSIER_REFERENCE = "DossierReference";
        private const string FASCICLE_REFERENCE = "FascicleReference";
        private const string OBJECT = "Object";
        #endregion

        #region [ Constructor ]
        public SkyDocCommandFormatter()
        {
            this._formatFragmentsValues = new Dictionary<string, Func<SkyDocCommand, string>>(StringComparer.InvariantCultureIgnoreCase);
            this._formatFragmentsValues.Add(DOSSIER_REFERENCE, (SkyDocCommand command) => command.DossierReference);
            this._formatFragmentsValues.Add(FASCICLE_REFERENCE, (SkyDocCommand command) => command.FascicleReference);
            this._formatFragmentsValues.Add(OBJECT, (SkyDocCommand command) => command.Object);
        }
        #endregion

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string[] formatFragments = format.Split(':');
            SkyDocCommand command = arg as SkyDocCommand;

            if (command == null)
            {
                throw new ArgumentNullException("SkyDocCommand cannot be null");
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
