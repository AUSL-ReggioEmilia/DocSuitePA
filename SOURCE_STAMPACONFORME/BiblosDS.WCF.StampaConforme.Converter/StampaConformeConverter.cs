using System;
using BiblosDS.WCF.Interface;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace BiblosDS.WCF.StampaConforme.Converter
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "StampaConformeConverter" in both code and config file together.
    public class StampaConformeConverter : IStampaConformeConverter
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StampaConformeConverter));

        public bool IsAlive()
        {
            return true;
        }

        public byte[] Convert(byte[] fileToConvert, string fileExtension)
        {
            try
            {
                logger.DebugFormat("Convert {0}", fileExtension);
                return ConverterAssemblyLoader.GetConverter().Convert(fileToConvert, fileExtension, "pdf", AttachConversionMode.Default);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }            
        }


        public string GetVersion()
        {
            return ConverterAssemblyLoader.GetConverter().GetVersion();
        }


        public byte[] ConvertWithParameters(byte[] fileToConvert, string fileExtension, AttachConversionMode mode)
        {
            try
            {
                logger.DebugFormat("ConvertWithParameters {0}, mode:{1}", fileExtension, mode);
                return ConverterAssemblyLoader.GetConverter().Convert(fileToConvert, fileExtension, "pdf", mode);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }           
        }
    }
}
