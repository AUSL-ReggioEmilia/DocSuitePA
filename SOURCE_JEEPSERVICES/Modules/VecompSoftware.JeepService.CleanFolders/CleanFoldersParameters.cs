using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class CleanFoldersParameters : JeepParametersBase
    {
        private IList<string> _foldersList = new List<string>();

        [Category("Folders")]
        [Description("Lista delle Directory (divise da pipe) dove devono essere eseguite le cancellazioni.")]
        [DisplayName("Directory da processare")]
        public string FoldersPath { get; set; }

        [Browsable(false)]
        public IList<string> FoldersList
        {
            get
            {
                if(String.IsNullOrEmpty(FoldersPath))
                    return new List<string>();

                if (!_foldersList.Any())
                {
                    try
                    {
                        _foldersList = new List<string>();
                        var spl = FoldersPath.Split('|');
                        foreach (var item in spl)
                        {
                            _foldersList.Add(item);
                        }
                    }
                    catch
                    {                        
                        return new List<string>();
                    }                    
                }
                return _foldersList;
            }
        }

        [Category("Folders")]
        [DefaultValue(FolderType.Simple)]
        [Description("Tipologia della directory da processare.")]
        [DisplayName("Tipo di Directory")]
        public FolderType FolderType { get; set; }

        [DefaultValue("Temp")]
        [Description("Cartella per elaborazione file temporaneri")]
        [Category("Folders")]
        [DisplayName("Directory temporanea")]
        public string TempFolder { get; set; }

        [Category("Params")]
        [DefaultValue("*.*")]
        [Description("Lista dei filtri (divisi da pipe) da utilizzare (attivo solo per il Tipo di directory Simple) es. '*.pdf|*.doc'.")]
        [DisplayName("Filtri")]
        public string Filters { get; set; }

        [Category("Params")]
        [DefaultValue(0)]
        [Description("Numero massimo di giorni da mantenere nelle Directory (se 0 verranno eliminati tutti i file).")]
        [DisplayName("Max numero giorni")]
        public int MaxNumberDaysToKeep { get; set; }        
    }
}
