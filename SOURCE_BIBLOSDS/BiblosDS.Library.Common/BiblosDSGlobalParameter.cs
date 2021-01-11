using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common.Enums; 

namespace BiblosDS.Library.Common
{
    /// <summary>
    /// classe singleton che carica dal database i parametri generali dell'installazione
    /// di BiblosDS
    /// </summary>
    class BiblosDSGlobalParameter
    {
        private static volatile BiblosDSGlobalParameter g_UniqueInstance = null;
        private static object syncObj = new object();

        private Dictionary<string, string> _parameters ;

        private BiblosDSGlobalParameter()
		{
            // TODO : li legge dal database 
            _parameters = new Dictionary<string, string>(1000); 
		}

        public static BiblosDSGlobalParameter GetUniqueInstance
        {
            get
            {
                if (g_UniqueInstance == null)
                {
                    lock (syncObj)
                    {
                        if (g_UniqueInstance == null)
                            g_UniqueInstance = new BiblosDSGlobalParameter();
                    }

                    return g_UniqueInstance;
                }
                else
                {
                    return g_UniqueInstance;
                }
            }
        }

        public string GetParameterValue(string ParameterName)
        {
            if (_parameters.ContainsKey(ParameterName) == true)
                return _parameters[ParameterName];
            else
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "BiblosDSGlobalParameter.GetParameterValue",
                                        "Parameter " + ParameterName + " is undefined",
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Managed_Error); 
                return null;
            }
        }
    }
}
