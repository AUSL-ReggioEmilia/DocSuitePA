using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Presentation;
using VecompSoftware.Helpers.Web;

namespace VecompSoftware.DocSuiteWeb.Facade.Report.Helpers
{
    /// <summary>
    /// Classe adibita allo scopo di convertire una lista di elementi in un dataset
    /// </summary>
    public static class DataSetConvertor
    {
        #region "Chiamate pubbliche"

        public static DataSet Convert<T>(IList<T> dataSetToConvert)
        {
            var dataSet = new DataSet();

            // Verifico se sia una RadGrid
            if (typeof(RadGrid).IsAssignableFrom(typeof(T)))
            {
                var grid = dataSetToConvert.FirstOrDefault() as RadGrid;
                if (grid != null)
                {
                    CreateDataSet(dataSet, grid.Columns);
                    FillDataSet(grid, dataSet);
                    return dataSet;
                }
            }

            // Gestisco altrimenti come lista generica
            CreateDataSet(dataSet, typeof(T));
            FillDataSet(typeof(T), dataSetToConvert.ToList(), dataSet);
            return dataSet;
        }


        #endregion

        #region "Gestione RadGrid"

        /// 
        /// Create the structure for all the tables in the data set        
        /// 
        /// Data set in which tables will be created
        /// Type of which dataset has to be created
        /// Whether current type is a child table        
        private static void CreateDataSet(DataSet dataSet, IEnumerable columns)
        {
            var dataTable = new DataTable("MainDataTable");

            // Create the structure for the data tables to be
            // added in the the data set            
            foreach (GridColumn column in columns)
            {
                // Se si tratta di un tipo complesso allora carico come String
                dataTable.Columns.Add(column.UniqueName.Replace(".", "_"), typeof(string));
            }

            //Add the table to the dataset
            dataSet.Tables.Add(dataTable);
        }

        /// 
        /// Fill all the tables of data set with data in the respective list        
        /// 
        /// Type of which datatable is to be filled
        /// List of data
        /// Data Set in which data tables will be filled with data    
        private static void FillDataSet(RadGrid radGrid, DataSet dataSet)
        {
            var dataTable = dataSet.Tables["MainDataTable"];
            var storedPaging = radGrid.AllowPaging;
            radGrid.AllowPaging = false;

            if (radGrid is BindGrid)
            {
                var bindGrid = radGrid as BindGrid;
                ((NHibernateProtocolFinder)bindGrid.Finder).EnablePaging = false;
                bindGrid.DataBindFinder();
            }
            
            foreach (GridDataItem item in radGrid.Items)
            {
                var row = dataTable.NewRow();
                foreach (GridColumn column in radGrid.Columns)
                {
                    var currentRadGridRow = item[column.UniqueName];
                    var value = currentRadGridRow.Text;
                    if (string.IsNullOrEmpty(value))
                    {
                        foreach (Control control in item[column.UniqueName].Controls)
                        {
                            if (control == null) continue;

                            // Verifico se label
                            var label = control as Label;
                            if (label != null)
                            {
                                value = label.Text;
                                break;
                            }

                            // Verifico se immagine
                            var image = control as Image;
                            if (image != null)
                            {
                                value = image.AlternateText;
                                break;
                            }

                            // Verifico se link
                            var linkButton = control as LinkButton;
                            if (linkButton != null)
                            {
                                value = linkButton.Text;
                                break;
                            }
                        }
                    }
                    // Rimuovo gli spazi bianchi eventuali
                    row[column.UniqueName.Replace(".", "_")] = value.Replace(WebHelper.Space, string.Empty);
                }
                dataTable.Rows.Add(row);
            }

            radGrid.AllowPaging = storedPaging;
            
            if (radGrid is BindGrid)
            {
                var bindGrid = radGrid as BindGrid;
                ((NHibernateProtocolFinder)bindGrid.Finder).EnablePaging = storedPaging;
                radGrid.DataSource = null;
            }

            // Ripristino lo status
            radGrid.Rebind();
        }

        #endregion

        #region "Gestione lista generica"

        /// 
        /// Create the structure for all the tables in the data set        
        /// 
        /// Data set in which tables will be created
        /// Type of which dataset has to be created
        /// Whether current type is a child table        
        private static void CreateDataSet(DataSet dataSet, Type type)
        {
            var dataTable = new DataTable(type.Name);

            // Create the structure for the data tables to be
            // added in the the data set            
            foreach (var pInfo in type.GetProperties())
            {
                // Se si tratta di un tipo complesso allora carico come String
                dataTable.Columns.Add(pInfo.PropertyType.GetGenericArguments().Length > 0
                                          ? new DataColumn(pInfo.Name, typeof(string))
                                          : new DataColumn(pInfo.Name, pInfo.PropertyType));
            }

            //Add the table to the dataset
            dataSet.Tables.Add(dataTable);
        }

        /// 
        /// Fill all the tables of data set with data in the respective list        
        /// 
        /// Type of which datatable is to be filled
        /// List of data
        /// Data Set in which data tables will be filled with data    
        private static void FillDataSet(Type type, IEnumerable list, DataSet dataSet)
        {
            var propertyInfos = type.GetProperties();
            var dataTable = dataSet.Tables[type.Name];

            foreach (var item in list)
            {
                var row = dataTable.NewRow();

                // Load all the data from the properties of the type
                // and save them into the datatable                
                foreach (var info in propertyInfos) row[info.Name] = info.GetValue(item, null);

                dataTable.Rows.Add(row);
            }
        }

        #endregion
    }
}
