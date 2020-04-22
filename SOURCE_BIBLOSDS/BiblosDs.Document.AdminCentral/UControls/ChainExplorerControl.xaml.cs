using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.UControls
{
    /// <summary>
    /// Interaction logic for ChainExplorerControl.xaml
    /// </summary>
    public partial class ChainExplorerControl : UserControl
    {
        ServiceReferenceContentSearch.ContentSearchClient search;
        ServiceReferenceDocument.DocumentsClient client;        
        public ChainExplorerControl()
        {
            InitializeComponent();
            search = new ServiceReferenceContentSearch.ContentSearchClient();
            client = new ServiceReferenceDocument.DocumentsClient();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            client.GetArchivesAsync();
            client.GetArchivesCompleted += (object o, GetArchivesCompletedEventArgs args) =>
            {
                if (args.Error == null)
                    ddlArchive.ItemsSource = args.Result;                    
                else
                    MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButton.OK);
            };
            
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            search.GetAllDocumentChainsAsync((ddlArchive.SelectedItem as Archive).Name, 0, 20);
            search.GetAllDocumentChainsCompleted += (object obj, ServiceReferenceContentSearch.GetAllDocumentChainsCompletedEventArgs args) =>
                {
                    if (args.Error == null)
                        gridDocuments.ItemsSource = args.Result;
                    else
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButton.OK);
                };
        }            
    }
}
