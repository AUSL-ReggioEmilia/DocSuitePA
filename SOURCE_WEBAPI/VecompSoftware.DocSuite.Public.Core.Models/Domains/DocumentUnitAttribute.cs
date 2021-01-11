using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DocumentUnitAttribute : Attribute
    {
        #region [ Fields ]

        private readonly string _documentUnitName = string.Empty;

        #endregion

        #region [ Contructors ]
        public DocumentUnitAttribute(DocumentUnitType documentUnitType, string documentUnitName = "")
        {
            DocumentUnitType = documentUnitType;
            switch (documentUnitType)
            {
                case DocumentUnitType.Invalid:
                    throw new ArgumentException("Tipologia di unità non valida");
                case DocumentUnitType.Generic:
                    {
                        _documentUnitName = "Generica";
                        break;
                    }
                case DocumentUnitType.Protocol:
                    {
                        _documentUnitName = "Protocollo";
                        break;
                    }
                case DocumentUnitType.Resolution:
                    {
                        _documentUnitName = "Delibera/Determina";
                        break;
                    }
                case DocumentUnitType.DocumentSeries:
                    {
                        _documentUnitName = "Serie documentale";
                        break;
                    }
                case DocumentUnitType.Archive:
                    {
                        _documentUnitName = "Archivio";
                        break;
                    }
                default:
                    throw new ArgumentException("Tipologia di unità non valida");
            }
            if (!string.IsNullOrEmpty(documentUnitName))
            {
                _documentUnitName = documentUnitName;
            }
        }
        #endregion

        #region [ Properties ]

        public DocumentUnitType DocumentUnitType { get; }

        public string DocumentUnitName => _documentUnitName;
        #endregion

    }
}
