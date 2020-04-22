using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Report
{
    public interface IReport<T>
    {
        /// <summary>
        /// Motore di report centrale
        /// </summary>
        ReportViewer TablePrint { get; }

        /// <summary>
        /// ?
        /// </summary>
        string TitlePrint { get; set; }
        
        /// <summary>
        /// Path del template Rdlc da utilizzare.
        /// Se presente significa che la stampa è di tipo canonico con template grafico.
        /// </summary>
        string RdlcPrint { get; set; }

        /// <summary>
        /// Definisce le colonne che devono essere utilizzate nella stampa del report (e relativi nomi).
        /// Se presente significa che la stampa è di tipo dinamico e sovrascrive il caricamento standard dei nomi di colonna.
        /// </summary>
        IList<Column> CustomColumns { get; set; }

        /// <summary>
        /// Definisce un DataSet da convertire
        /// </summary>
        IList<T> RawDataSet { get; set; }

        /// <summary>
        /// Passa 1 parametro al meccanismo di generazione del report
        /// </summary>
        /// <param name="name">Nome del parametro</param>
        /// <param name="value">Valore del parametro</param>
        void AddParameter(string name, string value);

        /// <summary>
        /// Aggiunge una lista di parametro al meccanismo di generazione del report
        /// </summary>
        /// <param name="parameters">Lista di parametri da passare</param>
        void AddParameters(Dictionary<string, string> parameters);

        /// <summary>
        /// Aggiunge gli elementi da stampare al meccanismo di generazione del report
        /// </summary>
        /// <param name="elements">Lista di elementi di tipo generico</param>
        void AddRange(IList<T> elements);

        /// <summary>
        /// Genera un PDF con nome standard (Report.pdf)
        /// </summary>
        /// <returns></returns>
        DocumentInfo DoPrint();

        /// <summary>
        /// Genera un PDF con uno specifico nome
        /// </summary>
        /// <param name="reportFileName">Nome del report</param>
        /// <returns></returns>
        DocumentInfo DoPrint(string reportFileName);

        /// <summary>
        /// Genera un file Excel con nome standard (Report.xls)
        /// </summary>
        /// <returns></returns>
        DocumentInfo ExportExcel();

        /// <summary>
        /// Genera un file Excel con uno specifico nome
        /// </summary>
        /// <param name="reportFileName">Nome del report</param>
        /// <returns></returns>
        DocumentInfo ExportExcel(string reportFileName);
    }
}
