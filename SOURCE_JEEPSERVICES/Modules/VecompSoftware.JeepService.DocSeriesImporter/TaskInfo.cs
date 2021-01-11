using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VecompSoftware.JeepService.DocSeriesImporter
{
    [XmlRootAttribute(ElementName = "taskInfo")]
    public class TaskInfo
    {
        [NonSerialized]
        private string sourceFile;

        public string documentFilename { get; set; }
        public int taskId { get; set; }
        public string taskCode { get; set; }
        public string taskError { get; set; }

        [XmlElementAttribute("rowInfo")]
        public List<RowInfo> rowList { get; set; }

        private Dictionary<string, RowInfo> rowDict = null;

        public TaskInfo()
        {
            documentFilename = "";
            taskId = 0;
            taskCode = "";
            rowList = new List<RowInfo>();
            taskError = "";
        }

        public static TaskInfo Load(string filename)
        {
            var res = XmlFile<TaskInfo>.Load(filename, "");
            res.sourceFile = filename;
            res.LoadDict();
            return res;
        }

        private void LoadDict()
        {
            rowDict = new Dictionary<string, RowInfo>();
            foreach (var row in rowList)
            {
                if (!rowDict.ContainsKey(row.id))
                    rowDict.Add(row.id, row);
            }
        }

        public static string[] GetInfoFiles(string dropFolder)
        {
            return new DirectoryInfo(dropFolder).GetFiles("*.xml")
              .OrderBy(f => f.CreationTime)
              .Select(f => f.FullName)
              .ToArray();
        }

        public void Save()
        {
            XmlFile<TaskInfo>.Serialize(this, sourceFile);
        }

        public void SaveAs(string filename)
        {
            sourceFile = filename;
            Save();
        }


        public bool HasErrors()
        {
            return rowList.Where(p => p.status == RowInfo.RowStatus.Error).Count() > 0;
        }

        public bool IsRowProcessed(string rowId)
        {
            if (rowDict.ContainsKey(rowId))
                return rowDict[rowId].status == RowInfo.RowStatus.Processed;

            return false;
        }

        public void AddRowInfo(string rowId, string msg, RowInfo.RowStatus rowStatus)
        {
            //contiene già la riga, ne faccio un update
            if (rowDict.ContainsKey(rowId))
            {
                RowInfo rowInfo = rowDict[rowId];
                rowInfo.message = msg;
                rowInfo.status = rowStatus;
            }
            else
            {
                var rowInfo = new RowInfo
                {
                    id = rowId,
                    message = msg,
                    status = rowStatus
                };

                rowList.Add(rowInfo);
                rowDict.Add(rowId, rowInfo);
            }            

            //salva il file
            Save();
        }


        public void RemoveFiles()
        {
            if (File.Exists(this.documentFilename))
                File.Delete(this.documentFilename);

            if (File.Exists(this.sourceFile))
                File.Delete(this.sourceFile);
        }

        public void CopyStatus(string statusFolder)
        {
            File.Copy(this.sourceFile, Path.Combine(statusFolder, Path.GetFileName(this.sourceFile)), true);
            File.Delete(this.sourceFile);
        }
    }


    public class RowInfo
    {
        public enum RowStatus
        {
            ToProcess,
            Processed,
            Error
        }

        public string id { get; set; }
        public RowStatus status { get; set; }
        public string message { get; set; }
    }

}
