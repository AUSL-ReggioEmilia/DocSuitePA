using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AmministrazioneTrasparente.Code
{
    public sealed class Singleton
    {
        #region [ Fields ]

        private const string DOCUMENTSERIES_HEADER_FILEPATH = "Config/DocumentSeriesHeader.json";
        private static Singleton _instance;
        private static object _syncRoot = new Object();
        private ICollection<DocumentSeriesHeader> _headerItems;

        #endregion

        #region [ Constructors ]

        private Singleton() { }

        #endregion

        #region [ Properties ]

        public static Singleton Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if(_instance == null)
                            _instance = new Singleton();
                    }                    
                }
                return _instance;
            }
        }

        public ICollection<DocumentSeriesHeader> DocumentSeriesHeaders
        {
            get
            {
                if(_headerItems == null)
                {
                    string headerItemFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DOCUMENTSERIES_HEADER_FILEPATH);
                    string headerItemContent = File.ReadAllText(headerItemFile, Encoding.UTF8);
                    _headerItems = JsonConvert.DeserializeObject<ICollection<DocumentSeriesHeader>>(headerItemContent);
                }
                return _headerItems;
            }
        }

        #endregion
    }
}