using System;
using System.Reflection;
using System.Configuration;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;

namespace BiblosDS.WCF.StampaConforme.Converter
{
    public class ConverterAssemblyLoader
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ConverterAssemblyLoader));
        private static ConverterAssemblyLoader istance;
        static object objLock = new object();
        IConverter converter;

        private ConverterAssemblyLoader()
        {
            logger.InfoFormat("ConverterAssemblyLoader {0}", ConfigurationManager.AppSettings["ConverterClassName"].ToString());
            Assembly a;
            a = Assembly.Load(new AssemblyName(ConfigurationManager.AppSettings["ConverterAssemblyName"].ToString()));
            converter = (IConverter)a.CreateInstance(ConfigurationManager.AppSettings["ConverterClassName"].ToString());
            if (converter == null)
                throw new Exception("Assembly converter not found");
        }

        public static IConverter GetConverter()
        {
            lock (objLock)
            {
                if (istance == null)
                    istance = new ConverterAssemblyLoader();
            }
            return istance.converter;
        }
    }
}