using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.StampaConforme.Models.Office.Outlook
{
    public class SaveMailToPdfRequest
    {
        public ConversionMode ConversionMode { get; set; }
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
    }
}
