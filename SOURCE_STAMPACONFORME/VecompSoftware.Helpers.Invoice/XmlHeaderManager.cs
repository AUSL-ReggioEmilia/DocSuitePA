using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace VecompSoftware.Helpers.Invoice
{
    public abstract class XmlHeaderManager
    {
        /// <summary>
        /// Namaspace del file di metadati. Il namespace deve essere specificato nel primo nodo del file.
        /// Il namespace viene riconosciuto attraverso il parametro <see cref="RegularExpressionToCheck"/>
        /// </summary>
        /// <value>The name space.</value>
        public virtual string NameSpace { get; set; }
        
        /// <summary>
        /// Versione del file di metatadi presente nell'attributo contente la descrizione presente in <see cref="StringVersion"/> 
        /// </summary>
        /// <value>The version.</value>
        public virtual double Version { get; set; }

        internal abstract string RegularExpressionToCheck { get; }

        internal abstract string StringVersion { get; }

        /// <summary>
        /// Ritorna il culture info utilizzata per il parsing della stringa testuale della versione.
        /// </summary>
        /// <value>CultureInfo</value>
        internal abstract string CultureInfoParsing { get; }
        
        private readonly string document;

        /// <summary>
        /// Manager per la verifica del documento
        /// </summary>
        /// <param name="document">Stringa contenente le informazioni del documento</param>
        public XmlHeaderManager(string document)            
        {
            this.document = document;
        }
        
        /// <summary>
        /// Viene controllato il primo nodo del documento.
        /// Viene verificato il namespace e la versione del file.
        /// </summary>
        public void Controlla()
        {
            string nS = string.Empty;
            double v = new double();
            try
            {
                var regXml = new Regex(RegularExpressionToCheck);

                using (XmlReader reader = XmlReader.Create(new StringReader(this.document)))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
                            {
                                reader.MoveToAttribute(attInd);
                                // CheckNameSpace
                                if (regXml.IsMatch(reader.Value))
                                {
                                    nS = reader.Value;
                                }
                                // CheckVersion
                                if (reader.Name.ToUpper().Contains(this.StringVersion))
                                {
                                    v = double.Parse(reader.Value, new System.Globalization.CultureInfo(CultureInfoParsing));
                                }
                            }
                            break;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
            }
            NameSpace = nS;
            Version = v;
        }
    }
}